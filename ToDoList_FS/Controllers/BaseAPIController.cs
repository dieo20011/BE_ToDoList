using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList_FS.Model;

namespace ToDoList_FS.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class BaseAPIController : ControllerBase
    {
        [NonAction]
        public IActionResult ErrorResult(string errMsg)
        {
            var dataResult = new ErrorResponeModel
            {
                Status = false,
                ErrorMessage = errMsg,
            };
            return Ok(dataResult);
        }
        /// <summary>
        /// prepare success result
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public IActionResult SuccessResult(string message)
        {
            var dataResult = new SuccessResponeModel<object>
            {
                Status = true,
                Message = message,
            };
            return Ok(dataResult);
        }


        /// <summary>
        /// prepare success result
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public IActionResult SuccessResult(object obj, string message = "")
        {
            var dataResult = new SuccessResponeModel<object>
            {
                Status = true,
                Message = message,
                Data = obj,
            };
            return Ok(dataResult);
        }
    }
}
