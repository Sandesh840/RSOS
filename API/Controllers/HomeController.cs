using Common.Utilities;
using System.Diagnostics;
using Application.DTOs.User;
using Application.DTOs.Error;
using Application.DTOs.Password;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using Edi.Captcha;
using Microsoft.Extensions.Caching.Memory;

namespace RSOS.Controllers;

public class HomeController : BaseController<HomeController>
{
    private Guid UserSessionId;
    private readonly IUserService _userService;
    private readonly IMemoryCache _memoryCache;
    private readonly ISessionBasedCaptcha _captcha;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(IUserService userService, IMemoryCache memoryCache, ISessionBasedCaptcha captcha, IWebHostEnvironment webHostEnvironment)
    {
        _captcha = captcha;
        _userService = userService;
        _memoryCache = memoryCache;
        _webHostEnvironment = webHostEnvironment;
    }

   
    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Index", "Notification");
        }
        if (HttpContext.Session.GetInt32("UserName") != null)
        {
            UserRequestDTO userRequestDTO = new UserRequestDTO();
            userRequestDTO.ExamUserName = HttpContext.Session.GetString("UserName");
        }

        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(UserRequestDTO userRequest)
    {
        if (string.IsNullOrEmpty(userRequest.UserName) || string.IsNullOrEmpty(userRequest.Password))
        {
            TempData["Warning"] = "Please insert your username and password before submitting your request.";

            return RedirectToAction("Login");
        }
        
        if (!_captcha.Validate(userRequest.Captcha, HttpContext.Session))
        {
            TempData["Warning"] = "Invalid captcha, please try again.";
                    
            return RedirectToAction("Login");
        }
           
        var isPasswordValid = await _userService.IsUserAuthenticated(userRequest);

        if (!isPasswordValid)
        {
            TempData["Warning"] = "Invalid username or password.";

            return RedirectToAction("Login");
        }
        
        var userId = await _userService.GetUserId(userRequest);

        var isExist = _memoryCache.TryGetValue(userId, out UserSessionId);

        if (isExist)
        {
            TempData["Warning"] = "The previous session was not logged out.";

            TempData["LogoutAlert"] = "The previous session was not logged out.";

            return RedirectToAction("Login");
        }

        HttpContext.Session.SetInt32("UserId", userId);
                    
        _memoryCache.Set(userId, UserSessionId);
            
        TempData["Success"] = "Successfully authenticated.";

        return RedirectToAction("Index", "Notification");
    }

    [Authentication]
    public ActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDTO changePassword)
    {
        var userId = HttpContext.Session.GetInt32("UserId") ?? 1;
        
        var isPasswordChanged = await _userService.ChangePassword(userId, changePassword.HdCurrentPassword, changePassword.HdNewPassword);

        if (isPasswordChanged)
        {
            TempData["Success"] = "Password successfully changed.";
            
            HttpContext.Session.Clear();
        
            HttpContext.Session.Remove("UserId");
        
            return RedirectToAction("Login");
        }

        TempData["Warning"] = "Please insert your correct current state of password before changing it.";

        return View(changePassword);
    }
    
    [HttpGet]
    public ActionResult Logout()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        
        _memoryCache.Remove(userId ?? 1);

        HttpContext.Session.Clear();
        
        HttpContext.Session.Remove("UserId");

        TempData["Success"] = "Successfully logged out.";

        return RedirectToAction("Login");
    }

    [Authentication]
    public IActionResult Enrollment()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
    
    [HttpGet]
    [Route("get-captcha-image")]
    public IActionResult GetCaptchaImage()
    {
        var captcha = _captcha.GenerateCaptchaImageFileStream(HttpContext.Session);
        
        return captcha;
    }
    
    public IActionResult DownloadApkFile()
    {
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "documents", "apk", "rsos.apk");
        
        if (!System.IO.File.Exists(filePath)) return NotFound();
        
        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        
        const string fileName = "RSOS_App.apk";
        
        return File(fileBytes, "application/vnd.android.package-archive", fileName);
    }
}