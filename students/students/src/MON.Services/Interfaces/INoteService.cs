namespace MON.Services.Interfaces
{
    using MON.Models.Grid;
    using MON.Models.NoteModels;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;

    public interface INoteService
    {
        Task<IPagedList<NoteViewModel>> List(NotesListInput input);
        Task<NoteModel> GetById(int noteId);

        Task CreateNote(NoteModel noteModel);


        Task Update(NoteModel noteModel);

        Task Delete(int id);
    }
}
