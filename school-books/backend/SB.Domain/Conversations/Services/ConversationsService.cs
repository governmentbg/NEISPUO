namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Data;
using static IConversationsQueryRepository;

internal class ConversationsService : IConversationsService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IConversationAggregateRepository conversationAggregateRepository;
    private readonly IConversationsQueryRepository conversationQueryRepository;

    public ConversationsService(
        IUnitOfWork unitOfWork,
        IConversationsQueryRepository conversationQueryRepository,
        IConversationAggregateRepository conversationAggregateRepository)
    {
        this.unitOfWork = unitOfWork;
        this.conversationQueryRepository = conversationQueryRepository;
        this.conversationAggregateRepository = conversationAggregateRepository;
    }

    public async Task<Conversation> CreateConversationAsync(
        CreateConversationCommand command,
        CancellationToken ct)
    {
        var now = DateTime.Now;
        var instId = command.InstId!.Value;
        var schoolYear = ConversationsHelper.CurrentSchoolYear();

        var conversation =
            new Conversation(
                schoolYear,
                command.Title!,
                command.IsLocked.GetValueOrDefault(),
                now);

        var participants =
            await this.GenerateParticipants(
                instId,
                schoolYear,
                command.Participants!,
                command.Creator!,
                conversation,
                ct);

        conversation.AddParticipants(participants);

        await this.conversationAggregateRepository.AddAsync(conversation, ct);

        var creator = participants.First(p => p.IsCreator);
        var newMessage = new ConversationMessage(
            schoolYear,
            conversation,
            creator.ConversationParticipantId,
            command.Message!,
            now);
        var message = await this.conversationAggregateRepository.AddAsync(newMessage, ct);
        conversation.UpdateLastReadMessage(message.ConversationMessageId, message.CreateDate);
        creator.UpdateLastReadMessage(message.ConversationMessageId, now);

        await this.unitOfWork.SaveAsync(ct);

        return conversation;
    }

    public async Task<GetConversationMessagesVO[]> GetConversationMessagesAsync(
        int sysUserId,
        int schoolYear,
        int conversationId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var messages = await this.conversationQueryRepository.GetConversationMessagesAsync(
            schoolYear,
            conversationId,
            offset,
            limit,
            ct);

        if (!messages.Any())
        {
            return messages;
        }

        if (offset == null || offset == 0)
        {
            var conversation = await this.conversationAggregateRepository.FindAsync(
                schoolYear,
                conversationId,
                ct);

            var message = messages.First();

            conversation.UpdateParticipantLastReadMessage(sysUserId, message.MessageId, message.CreateDate);
            await this.unitOfWork.SaveAsync(ct);
        }

        return messages;
    }

    private async Task<ConversationParticipant[]> GenerateParticipants(
        int instId,
        int schoolYear,
        CreateConversationCommandParticipant[] participants,
        CreateConversationCommandCreator creator,
        Conversation conversation,
        CancellationToken ct)
    {
        // Get creator participant
        var creatorParticipant = await this.CreateCreatorParticipant(instId, schoolYear, creator, conversation, ct);

        var participantsList = new List<ConversationParticipant> { creatorParticipant };

        foreach (var participant in participants)
        {
            switch (participant.ParticipantType)
            {
                case ParticipantType.ParentsForClass:
                    var parentParticipants = await this.CreateParentTypeParticipants(instId, schoolYear, participant, conversation, ct);
                    participantsList.AddRange(parentParticipants);
                    break;

                case ParticipantType.TeachersForClass:
                    var teacherParticipants = await this.CreateTeacherTypeParticipants(instId, schoolYear, participant, conversation, ct);
                    participantsList.AddRange(teacherParticipants);
                    break;

                case ParticipantType.Parent:
                case ParticipantType.Teacher:
                    var regularParticipant = new ConversationParticipant(
                        conversation,
                        schoolYear,
                        instId,
                        participant.SysUserId.GetValueOrDefault(),
                        participant.Title!,
                        participant.ParticipantType.GetValueOrDefault());
                    participantsList.Add(regularParticipant);
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(ParticipantType));
            }
        }

        // Get class leaders
        var parentSysUserIds = participantsList
            .Where(p => p.ParticipantType == ParticipantType.Parent)
            .Select(p => p.SysUserId)
            .ToArray();
        var teacherSysUserIds = participantsList
            .Where(p => p.ParticipantType == ParticipantType.Teacher)
            .Select(p => p.SysUserId)
            .ToArray();
        var classLeaders = await this.conversationQueryRepository.GetClassLeadersAsync(instId, parentSysUserIds, teacherSysUserIds, ct);
        foreach (var classLeader in classLeaders)
        {
            var existingTeacher = participantsList.FirstOrDefault(p => p.SysUserId == classLeader.SysUserId);
            var isCreator = false;
            if (existingTeacher != null)
            {
                isCreator = existingTeacher.IsCreator;
                participantsList.Remove(existingTeacher);
            }

            participantsList.Add(
                new ConversationParticipant(
                    conversation,
                    schoolYear,
                    instId,
                    classLeader.SysUserId!.Value,
                    classLeader.Title,
                    ParticipantType.Teacher,
                    isCreator: isCreator
                ));
        }

        // Update participants info
        var participantsInfoTitle = new List<string>();
        if (classLeaders.Any(cl => cl.SysUserId == creatorParticipant.SysUserId))
        {
            participantsInfoTitle.Add(classLeaders.FirstOrDefault(cl => cl.SysUserId == creatorParticipant.SysUserId)!.Title);
        }
        else
        {
            participantsInfoTitle.Add(creatorParticipant.Title);
        }

        participantsInfoTitle.AddRange(participants.Select(p => p.Title)!);
        participantsInfoTitle.AddRange(classLeaders.Where(cl => cl.SysUserId != creatorParticipant.SysUserId).Select(cl => cl.Title));
        conversation.UpdateParticipantsInfo(string.Join(", ", participantsInfoTitle.Distinct()));

        return participantsList
            .OrderBy(p => p.ParticipantType)
            .ThenBy(p => p.Title)
            .ToArray();
    }

    private async Task<ConversationParticipant> CreateCreatorParticipant(
        int instId,
        int schoolYear,
        CreateConversationCommandCreator creator,
        Conversation conversation,
        CancellationToken ct)
    {
        var participantType = this.GetParticipantType(creator.SysRoleId);

        var creatorInfo = await this.conversationQueryRepository.GetCreatorParticipantInfoAsync(
            instId,
            creator.SysUserId,
            participantType,
            ct);

        return new ConversationParticipant(
            conversation,
            schoolYear,
            instId,
            creator.SysUserId,
            $"{creatorInfo.Title} ({creatorInfo.ParticipantType.GetEnumDescription()})",
            participantType,
            isCreator: true);
    }

    private async Task<ConversationParticipant[]> CreateTeacherTypeParticipants(
        int instId,
        int schoolYear,
        CreateConversationCommandParticipant participant,
        Conversation conversation,
        CancellationToken ct)
    {
        var group = new ConversationParticipantGroup(
            schoolYear,
            participant.Title!,
            participant.ParticipantType.GetValueOrDefault(),
            participant.ClassBookId);

        var teachers =
            await this.conversationQueryRepository.GetTeachersParticipantsAsync(
            instId,
            participant.ClassBookId,
            ct);

        return teachers.Select(t => new ConversationParticipant(
                conversation,
                schoolYear,
                instId,
                t.SysUserId.GetValueOrDefault(),
                t.Title,
                t.ParticipantType,
                group))
            .ToArray();
    }

    private async Task<ConversationParticipant[]> CreateParentTypeParticipants(
        int instId,
        int schoolYear,
        CreateConversationCommandParticipant participant,
        Conversation conversation,
        CancellationToken ct)
    {
        var group = new ConversationParticipantGroup(
            schoolYear,
            participant.Title!,
            participant.ParticipantType.GetValueOrDefault(),
            participant.ClassBookId);

        var parents =
            await this.conversationQueryRepository.GetParentParticipantsAsync(
                instId,
                participant.ClassBookId,
                ct);

        return parents.Select(t => new ConversationParticipant(
                conversation,
                schoolYear,
                instId,
                t.SysUserId.GetValueOrDefault(),
                t.Title,
                t.ParticipantType,
                group))
            .ToArray();
    }

    private ParticipantType GetParticipantType(SysRole selectedRoleId)
        => selectedRoleId switch
        {
            SysRole.Teacher => ParticipantType.Teacher,
            SysRole.Parent => ParticipantType.Parent,
            SysRole.Institution or SysRole.InstitutionExpert => ParticipantType.Admin,
            _ => throw new InvalidEnumArgumentException(nameof(ParticipantType))
        };
}
