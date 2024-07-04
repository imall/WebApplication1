using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ContosoUniversityContext _context;

    public WeatherForecastController(ContosoUniversityContext context)
    {
        _context = context;
    }

    /// <summary>
    /// ���X�Ҧ��Ҧ����
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetCourse")]
    public IActionResult GetCourse()
    {
        var course = _context.Courses.ToList();
        return Ok(course);
    }

    /// <summary>
    /// ²��d�߻y�k
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [HttpGet("GetCourseIdLargeThanFive")]
    public IActionResult GetCourseIdLargeThanFive(int? number)
    {
        var course = _context.Courses.Where(x => x.CourseId > number);


        return Ok(course);
    }

    /// <summary>
    /// �� linq �g�d�߻y�k
    /// </summary>
    /// <param name="something"></param>
    /// <returns></returns>
    [HttpGet("GetCourseContainSomeThing")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public IActionResult GetCourseIdLargeThanFive(string something)
    {
        if (string.IsNullOrEmpty(something))
        {
            return BadRequest();
        }

        //var course = context.Courses.Where(x => x.Title.Contains(something));

        var course = from c in _context.Courses
                     where c.Title.Contains(something)
                     select c;

        return Ok(course);
    }


    /// <summary>
    /// join course �� department �H departmentID �� key
    /// </summary>
    /// <param name="something"></param>
    /// <returns></returns>
    [HttpGet("GetCourseJoin")]
    public IActionResult GetCourseJoinDepartment(string something = "Git")
    {
        if (string.IsNullOrEmpty(something))
        {
            return BadRequest();
        }

        // �� linq �g join
        var linqJoin = from c in _context.Courses
                       join d in _context.Departments on c.DepartmentId equals d.DepartmentId
                       where c.Title.Contains(something)
                       select new
                       {
                           c.CourseId,
                           c.Title,
                           DepartmentName = d.Name
                       };

        // �� lambda �g join
        var lambdaJoin = _context.Courses
            .Include(c => c.Department)
            .Where(c => c.Title.Contains(something))
            .Select(cd => new
            {
                cd.CourseId,
                cd.Title,
                DepartmentName = cd.Department.Name
            });


        return Ok(lambdaJoin);
    }

    /// <summary>
    /// �_�l�P�������P�_
    /// </summary>
    /// <param name="something"></param>
    /// <returns></returns>
    [HttpGet("GetCourseStartAndEnd")]
    public IActionResult GetCourseStartAndEnd(string something = "Git")
    {
        if (string.IsNullOrEmpty(something))
        {
            return BadRequest();
        }

        var course = _context.Courses.Where(x => x.Title.StartsWith(something) || x.Title.EndsWith(something));

        // var course = from c in context.Courses
        //              where c.Title.StartsWith(something) || c.Title.EndsWith(something)
        //              select c;


        return Ok(course);
    }

    /// <summary>
    /// ����Ƥ����ܽd
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetCoursePage/{pageIndex:int}/{pageSize:int}")]
    public async Task<IActionResult> GetCoursePage(int pageIndex = 1, int pageSize = 10)
    {
        // �Ƨ�
        var course = _context.Courses.OrderBy(x => x.CourseId).AsQueryable();

        // �p���`����
        var total = _context.Courses.Count();

        // �p���e���ƪ����
        var data = await course.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new { total, data });
    }
    
    /// <summary>
    /// �妸��s Course �� Credits ���
    /// </summary>
    /// <returns></returns>
    [HttpPost("BatchUpdateCredits")]
    public async Task<IActionResult> BatchUpdateCredits()
    {
        await _context.Courses.ExecuteUpdateAsync(setter =>
            setter.SetProperty(c => c.Credits, c => c.Credits + 1)
        );

        return NoContent();
    }
}
