using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes.API.Data;
using Notes.API.Models.Entities;

namespace Notes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        private readonly NotesDbContext _notesDbContext;
        public NotesController(NotesDbContext notesDbContext)
        {
            _notesDbContext = notesDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            //get notes from database
            return Ok(await _notesDbContext.Notes.ToListAsync());

        }
        [HttpGet]
        [Route("{id:Guid}")]
        [ActionName("GetNoteById")]
        public async Task<IActionResult> GetNoteById([FromRoute] Guid id)
        {
            //await _notesDbContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
            //or
            var note = await _notesDbContext.Notes.FindAsync(id);
            if(note == null)
            {
                return NotFound();
            }
            return Ok(note);
        }

        [HttpPost]

        public async Task<IActionResult> AddNote(Note note)
        {
            note.Id = new Guid();
            await _notesDbContext.Notes.AddAsync(note);
            await _notesDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNoteById), new {id = note.Id}, note);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateNote([FromRoute] Guid id, [FromBody] Note updatedNote)
        {
            var existingNote = await _notesDbContext.Notes.FindAsync(id);
            if(existingNote == null)
            {
                return NotFound();
            }
            existingNote.Title = updatedNote.Title;
            existingNote.Description = updatedNote.Description;
            existingNote.IsVisible = updatedNote.IsVisible;

            await _notesDbContext.SaveChangesAsync();
            return Ok(existingNote);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteNote([FromRoute] Guid id)
        {
            var existingNote = await _notesDbContext.Notes.FindAsync(id);
            if(existingNote == null)
            {
                return NotFound();
            }
            _notesDbContext.Notes.Remove(existingNote);
            await _notesDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
