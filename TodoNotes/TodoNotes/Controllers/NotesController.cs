﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoNotes.Models;

namespace TodoNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly TodoNotesContext _context;

        public NotesController(TodoNotesContext context)
        {
            _context = context;
        }

        // GET: api/Notes
        [HttpGet]
        public async Task<IActionResult> GetTodoNotes()
        {
            return Ok(_context.Notes.Include(p => p.checkList).Include
               (p => p.label));

        }

        // GET: api/Notes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoNotes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var todoNotes = await _context.TodoNotes.FindAsync(id);


            IEnumerable<Notes> todoNotes = _context.Notes.Include(p => p.checkList).Include
              (p => p.label).Where(p => p.Id == id);
            if (todoNotes == null)
            {
                return NotFound();
            }

            return Ok(todoNotes);
        }
        [HttpGet("{label}")]
        public async Task<IActionResult> GetTodoNotes([FromQuery] string Label)
        {
            IEnumerable<Notes> todoNotes = _context.Notes.Include(p => p.checkList).Include
                (p => p.label).Where(p => p.label != null);
            return Ok(todoNotes.Where(p => p.label.Any(q => q.LabelName == Label)));
        }
        [HttpGet("pin/{PinnedStatus}")]
        public async Task<IActionResult> GetTodoNotes([FromRoute] bool PinnedStatus)
        {
            IEnumerable<Notes> todoNotes = _context.Notes.Include(p => p.checkList).Include(p => p.label).Where(p => p.PinStatus == PinnedStatus);
            return Ok(todoNotes);
        }
        [HttpGet("title/{title}")]
        public async Task<IActionResult> GetTodoNotesByTitle([FromRoute] string title)
        {
            IEnumerable<Notes> todoNotes = _context.Notes.Include(p => p.checkList).Include(p => p.label).Where(p => p.Title == title);
            return Ok(todoNotes);

        }

        // PUT: api/Notes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoNotes([FromRoute] int id, [FromBody] Notes todoNotes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != todoNotes.Id)
            {
                return BadRequest();
            }

            // _context.Entry(todoNotes).State = EntityState.Modified;
            _context.Notes.Update(todoNotes);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Notes
        [HttpPost]
        public async Task<IActionResult> PostTodoNotes([FromBody] Notes todoNotes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Notes.Add(todoNotes);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoNotes", new { id = todoNotes.Id }, todoNotes);
        }


        // DELETE: api/Notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoNotes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var todoNotes = await _context.Notes.FindAsync(id);
            if (todoNotes == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(todoNotes);
            await _context.SaveChangesAsync();

            return Ok(todoNotes);
        }

        private bool NotesExists(int id)
        {
            return _context.Notes.Any(e => e.Id == id);
        }
    }
}