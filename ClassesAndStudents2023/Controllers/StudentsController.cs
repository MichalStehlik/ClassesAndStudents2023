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
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private ILogger<StudentsController> _logger;

        public StudentsController(ApplicationDbContext context, ILogger<StudentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // List
        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents(
            string? firstname,
            string? lastname,
            string? search,
            int? classroom,
            string? order
            )
        {
            _logger.LogInformation("Fetching Students");
            IQueryable<Student> students = _context.Students;

            if (!String.IsNullOrEmpty(firstname))
            {
                students = students.Where(s => s.Firstname.Contains(firstname));
            };
            if (!String.IsNullOrEmpty(lastname))
            {
                students = students.Where(s => s.Lastname.Contains(lastname));
            };
            if (!String.IsNullOrEmpty(search))
            {
                students = students.Where(s => s.Lastname.Contains(search) || s.Firstname.Contains(search));
            };
            if (classroom != null)
            {
                students = students.Where(s => s.ClassroomId == classroom);
            };

            if (!String.IsNullOrEmpty(order))
            {
                switch(order)
                {
                    case "firstname": students = students.OrderBy(s => s.Firstname); break;
                    case "firstname_desc": students = students.OrderByDescending(s => s.Firstname); break;
                    default: break;
                }
            }

            return await students.ToListAsync();
        }

        // detail
        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                _logger.LogError("Requested student " + id + " not found.");
                return NotFound();
            }

            return student;
        }

        // Replace
        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent([FromBody] Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.StudentId }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound("Student does not exist.");
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
