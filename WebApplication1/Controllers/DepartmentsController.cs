using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosoUniversityContext _context;

        public DepartmentsController(ContosoUniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments/5/Course
        /// <summary>
        /// 示範循環參考問題
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourseByDepartmentId(int id)
        {
            var department = await _context.Departments.AsNoTracking()
                .Include(d => d.Courses)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
            {
                return NotFound();
            }
            
            return Ok(department.Courses);
        }
       
    }
}
