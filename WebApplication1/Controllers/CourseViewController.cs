using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseViewController : ControllerBase
{
    private readonly ContosoUniversityContext _context;

    public CourseViewController(ContosoUniversityContext context)
    {
        _context = context;
    }

    [HttpGet("GetMyCourseView")]
    public IActionResult GetMyCourseView()
    {
        var course = _context.MyCourseViews;
        return Ok(course);
    }


    [HttpGet("GetCoursesByTitle")]
    public async Task<IActionResult> GetCoursesByTitleStartOrEnd(string q)
    {
        Console.WriteLine(q);
        
        var data = await _context.GetProcedures().GetCoursesByTitleStartOrEndAsync(q);
        return Ok(data);
    }

}