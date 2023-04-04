using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassesAndStudents2023.Data;
using ClassesAndStudents2023.Models;

namespace ClassesAndStudents2023.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClassroomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Classrooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Classroom>>> GetClassrooms()
        {
            return await _context.Classrooms.Include(c => c.Students).ToListAsync();
        }

        // GET: api/Classrooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Classroom>> GetClassroom(int id)
        {
            var classroom = await _context.Classrooms.FindAsync(id);

            if (classroom == null)
            {
                return NotFound();
            }

            return classroom;
        }

        // PUT: api/Classrooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClassroom(int id, Classroom classroom)
        {
            if (id != classroom.ClassroomId)
            {
                return BadRequest();
            }

            _context.Entry(classroom).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassroomExists(id))
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

        // POST: api/Classrooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Classroom>> PostClassroom(Classroom classroom)
        {
            _context.Classrooms.Add(classroom);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClassroom", new { id = classroom.ClassroomId }, classroom);
        }

        // DELETE: api/Classrooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassroom(int id)
        {
            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom == null)
            {
                return NotFound();
            }

            _context.Classrooms.Remove(classroom);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/students")]
        public async Task<ActionResult<ICollection<Student>>> GetClassroomStudents(int id)
        {
            var classroom = await _context.Classrooms.FindAsync(id);

            if (classroom == null)
            {
                return NotFound("classroom does not exists");
            }

            _context.Entry(classroom).Collection(c => c.Students).Load();

            return Ok(classroom.Students);
        }

        [HttpPost("{id}/students")]
        public async Task<ActionResult> AddClassroomStudent(int classroomId, int studentId)
        {
            var classroom = await _context.Classrooms.FindAsync(classroomId);

            if (classroom == null)
            {
                return NotFound("classroom does not exists");
            }

            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return NotFound("student does not exists");
            }

            classroom.Students.Add(student);
            _context.SaveChanges();

            return Ok();
        }
        
        [HttpDelete("{id}/students/{studentId}")]
        public async Task<IActionResult> DeleteClassroomStudent(int id, int studentId)
        {
            if (_context.Classrooms == null)
            {
                return NotFound();
            }
            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom == null)
            {
                return NotFound("class not found");
            }
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return NotFound("student not found");
            }
            await _context.Entry(classroom).Collection(g => g.Students).LoadAsync();
            if (classroom.Students.All(c => c.StudentId != studentId))
            {
                return NotFound("student is not in this class");
            }
            game.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClassroomExists(int id)
        {
            return _context.Classrooms.Any(e => e.ClassroomId == id);
        }
    }
}
