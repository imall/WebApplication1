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
    /// 取出所有所有資料
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetCourse")]
    public IActionResult GetCourse()
    {
        var course = _context.Courses.ToList();
        return Ok(course);
    }

    /// <summary>
    /// 簡單查詢語法
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
    /// 用 linq 寫查詢語法
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
    /// join course 跟 department 以 departmentID 當 key
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

        // 用 linq 寫 join
        var linqJoin = from c in _context.Courses
                       join d in _context.Departments on c.DepartmentId equals d.DepartmentId
                       where c.Title.Contains(something)
                       select new
                       {
                           c.CourseId,
                           c.Title,
                           DepartmentName = d.Name
                       };

        // 用 lambda 寫 join
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
    /// 起始與結束的判斷
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
    /// 取資料分頁示範
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetCoursePage/{pageIndex:int}/{pageSize:int}")]
    public async Task<IActionResult> GetCoursePage(int pageIndex = 1, int pageSize = 10)
    {
        // 排序
        var course = _context.Courses.OrderBy(x => x.CourseId).AsQueryable();

        // 計算總筆數
        var total = _context.Courses.Count();

        // 計算當前頁數的資料
        var data = await course.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new { total, data });
    }
    
    /// <summary>
    /// 批次更新 Course 的 Credits 欄位
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
