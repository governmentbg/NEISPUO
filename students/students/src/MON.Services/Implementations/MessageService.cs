namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models;
    using MON.Services.Interfaces;
    using MON.Shared;
    using MON.Shared.Interfaces;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Linq.Dynamic.Core;
    using System.Collections.Generic;
    using MON.Shared.ErrorHandling;

    public class MessageService : BaseService<MessageService>, IMessageService
    {
        private readonly ISignalRNotificationService _signalRNotificationService;
        private readonly IBlobService _blobService;

        public MessageService(DbServiceDependencies<MessageService> dependencies,
            IBlobService blobService,
            ISignalRNotificationService signalRNotificationService
            )
            : base(dependencies)
        {
            _signalRNotificationService = signalRNotificationService;
            _blobService = blobService;
        }


        #region Private members

        public void CheckPermisson(Message message)
        {
            if (message == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (message.ReceiverId != _userInfo.SysUserID && message.SenderId != _userInfo.SysUserID)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }
        }

        public void CheckPermisson(MessageViewModel message)
        {
            if (message == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (message.ReceiverId != _userInfo.SysUserID && message.SenderId != _userInfo.SysUserID)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }
        }

        private async Task<string> GetPersonUsernameAsync(int personId)
        {
            var name = await _context.Messages
             .Where(m => m.SenderId == personId)
             .Select(m => m.Receiver.Username)
             .FirstOrDefaultAsync();

            return name;
        }
        #endregion

        public async Task<int> SendMessageAsync(MessageModel messageModel)
        {
            Message entry = new Message
            {
                SenderId = _userInfo.SysUserID,
                ReceiverId = messageModel.ReceiverId,
                Contents = messageModel.Content,
                Subject = messageModel.Subject,
                SendDate = DateTime.UtcNow,
            };

            _context.Messages.Add(entry);

            var personNotReadMessagesCount =  await _context.Messages
             .CountAsync(m => m.ReceiverId == messageModel.ReceiverId && !m.IsArchived && !m.IsDeleted && !m.IsRead);
            var personUsername = await GetPersonUsernameAsync(messageModel.ReceiverId);
            await _signalRNotificationService.ChangePersonMessages(personUsername, personNotReadMessagesCount);

            await SaveAsync();

            return entry.Id;
        }

        public async Task<IPagedList<MessageViewModel>> ListMy(MessagesListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            IQueryable<Message> query = _context.Messages
                .AsNoTracking()
                .Where(m => m.ReceiverId == _userInfo.SysUserID && !m.IsArchived && !m.IsDeleted);

            if (!input.Filter.IsNullOrWhiteSpace())
            {
                query = query.Where(x => x.Subject.Contains(input.Filter)
                    || x.Sender.Person.FirstName.Contains(input.Filter)
                    || x.Sender.Person.LastName.Contains(input.Filter));
            }

            IQueryable<MessageViewModel> listQuery = query
                .Select(m => new MessageViewModel
                {
                    Id = m.Id,
                    SenderName = m.Sender.Person.FirstName + " " + m.Sender.Person.LastName,
                    Content = m.Contents,
                    Subject = m.Subject,
                    SendDate = m.SendDate,
                    IsRead = m.IsRead,
                    Attachment = m.MessageAttachments.Select(ma => new MessageAttachmentViewModel
                    {
                        FileName = ma.FileName,
                        ContentType = ma.ContentType
                    }).ToList()
                })
                .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "Id desc" : input.SortBy); ;

            int totalCount = listQuery.Count();
            IList<MessageViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<MessageViewModel> GetById(int messageId)
        {
            MessageViewModel message =  await _context.Messages
                .Where(m => m.Id == messageId && !m.IsDeleted && !m.IsArchived)
                .Select(m => new MessageViewModel
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.Person.FirstName + " " + m.Sender.Person.LastName,
                    ReceiverId = m.ReceiverId,
                    Subject = m.Subject,
                    Content = m.Contents,
                    SendDate = m.SendDate,
                    IsRead = m.IsRead,
                    Attachment = m.MessageAttachments.Select(ma => new MessageAttachmentViewModel
                    {
                        FileName = ma.FileName,
                        ContentType = ma.ContentType
                    })
                    .ToList()
                }).SingleOrDefaultAsync();

            CheckPermisson(message);

            return message;
        }

        public async Task<int> CountMyUnreadMessages()
        {
            return await _context.Messages
             .CountAsync(m => m.ReceiverId == _userInfo.SysUserID && !m.IsArchived && !m.IsDeleted && !m.IsRead);
        }

        public async Task MarkAsRead(int messageId)
        {
            var entity = await _context.Messages
                .Where(m => m.Id == messageId)
                .SingleOrDefaultAsync();

            CheckPermisson(entity);

            if (entity.ReceiverId != _userInfo.SysUserID)
            {
                // Не е моя и нямам права
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!entity.IsRead)
            {
                entity.IsRead = true;
                entity.ReadDate = DateTime.UtcNow;

                await SaveAsync();
            }
        }

        public async Task ArchiveAsync(int messageId)
        {
            Message entity = await _context.Messages
                .Where(m => m.Id == messageId)
                .SingleOrDefaultAsync();

            CheckPermisson(entity);

            if (entity.ReceiverId != _userInfo.SysUserID)
            {
                // Не е моя и нямам права
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.IsArchived = true;
            await SaveAsync();
        }

        public async Task DeleteAsync(int messageId)
        {
            Message entity = await _context.Messages
                .Where(m => m.Id == messageId)
                .SingleOrDefaultAsync();

            CheckPermisson(entity);

            if (entity.ReceiverId != _userInfo.SysUserID)
            {
                // Не е моя и нямам права
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.IsDeleted = true;
            await SaveAsync();
        }

        public async Task DeleteSelected(SelectedEntities model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (model.Ids.IsNullOrEmpty())
            {
                return;
            }

            List<Message> messages = await _context.Messages
                .Where(x => model.Ids.Contains(x.Id))
                .ToListAsync();

            foreach (var msg in messages)
            {
                CheckPermisson(msg);
                if (msg.ReceiverId != _userInfo.SysUserID)
                {
                    // Не е моя и нямам права
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }

                msg.IsDeleted = true;
            }

            await SaveAsync();
        }

        public async Task ArchiveSelected(SelectedEntities model)
        {
            if(model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (model.Ids.IsNullOrEmpty())
            {
                return;
            }

            List<Message> messages = await _context.Messages
                .Where(x => model.Ids.Contains(x.Id))
                .ToListAsync();

            foreach (var msg in messages)
            {
                CheckPermisson(msg);
                if (msg.ReceiverId != _userInfo.SysUserID)
                {
                    // Не е моя и нямам права
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }

                msg.IsArchived = true;
            }

            await SaveAsync();
        }

        //private async Task ProcessAddedDoc(List<MessageAttachmentModel> attachments, Message entry)
        //{
        //    if (attachments == null || !attachments.Any() || entry == null) return;

        //    foreach (var att in attachments)
        //    {
        //        var result = await _blobService.UploadFileAsync(att.NoteContents, att.FileName, att.ContentType);

        //        entry.MessageAttachments.Add(new MessageAttachment
        //        {
        //            BlobId = result.BlobId,
        //            FileName = att.FileName,
        //            FileSize = att.FileSize,
        //            ContentType = att.ContentType
        //        });
        //    }
        //}
    }
}
