using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MedicalEquipmentProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceNoteApiController : ControllerBase
    {
        private static List<DeviceNote> notes = new List<DeviceNote>();
        private static int nextId = 1;

        [HttpGet]
        public IActionResult Get() => Ok(notes);

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var note = notes.FirstOrDefault(n => n.Id == id);
            return note == null ? NotFound() : Ok(note);
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public IActionResult Create([FromBody] DeviceNote note)


        {
            Console.WriteLine("POST được gọi - " + HttpContext.Request.Path);

            note.Id = nextId++;
            notes.Add(note);
            return CreatedAtAction(nameof(Get), new { id = note.Id }, note);


        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] DeviceNote updatedNote)
        {
            var note = notes.FirstOrDefault(n => n.Id == id);
            if (note == null) return NotFound();
            note.Content = updatedNote.Content;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var note = notes.FirstOrDefault(n => n.Id == id);
            if (note == null) return NotFound();
            notes.Remove(note);
            return NoContent();
        }

        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            return Ok("API hoạt động");
        }

    }

    public class DeviceNote
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }
}
