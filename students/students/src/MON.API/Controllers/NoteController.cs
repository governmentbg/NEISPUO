namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models.Grid;
    using MON.Models.NoteModels;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;

    public class NoteController : BaseApiController
    {
        private readonly INoteService _noteServiceInterface;

        public NoteController(INoteService noteServiceInterface)
        {
            _noteServiceInterface = noteServiceInterface;
        }

        [HttpGet]
        public async Task<NoteModel> GetById(int id)
        {
            return await _noteServiceInterface.GetById(id);
        }

        [HttpGet]
        public async Task<IPagedList<NoteViewModel>> List([FromQuery] NotesListInput input)
        {
            var result = await _noteServiceInterface.List(input);

            return result;
        }

        [HttpPost]
        public async Task Create(NoteModel model)
        {
            await _noteServiceInterface.CreateNote(model);
        }

        [HttpPut]
        public async Task Update(NoteModel model)
        {
            await _noteServiceInterface.Update(model);
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            await _noteServiceInterface.Delete(id);
        }
    }
}
