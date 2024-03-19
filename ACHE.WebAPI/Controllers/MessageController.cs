using ACHE.Extensions;
using ACHE.Model;
using ACHE.Model.Negocio.Mensajes;
using ACHE.Negocio.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Http;

namespace ACHE.WebAPI.Controllers
{
    [RoutePrefix("mensajes")]
    public class MessageController : ApiController
    {
        private const int AUTH_USER_ID = 3155; //Id del usuario autorizado a agregar mensajes
        private const int MSGTYPE_BASE = 0x00;
        private const int MSGTYPE_NOTICE = MSGTYPE_BASE;
        private const int MSGTYPE_PUBLIC = MSGTYPE_BASE | 0x01;
        private const int MSGTYPE_PRIVATE = MSGTYPE_BASE | 0x02;
        private static readonly Expression<Func<Message, MessageDto>> _asMsgDto =
            x => new MessageDto
            {
                Id = x.Id,
                Date = x.Date,
                Type = x.Type,
                Email = x.Email,
                Body = x.Body
            };
        private readonly ACHEEntities _dbContext = new ACHEEntities();
        [Route("")]
        public IHttpActionResult GetPublic([FromUri] string email, [FromUri] DateTime pubdate, [FromUri] DateTime privdate)
        {
            if (email is string eml && !eml.IsValidEmailAddress())
                return StatusCode(HttpStatusCode.Forbidden);
            if (!_dbContext.Usuarios.Any(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)))
                return NotFound();
            var msgs = new List<MessageDto>();
            if (_dbContext.Message
                .Where(m => m.Type == MSGTYPE_PRIVATE && m.Date > privdate && m.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).ToList() is List<Message> msglst && msglst.Any())
                msgs.Add(new MessageDto
                {
                    Id = 0,
                    Type = MSGTYPE_NOTICE,
                    Date = DateTime.Now,
                    Body = $"Tiene {msglst.Count} mensaje{(msglst.Count > 1 ? "s" : string.Empty)} privados."
                });
            msgs.AddRange(_dbContext.Message.Where(m => m.Type == MSGTYPE_PUBLIC && m.Date > pubdate).Select(_asMsgDto));
            return Ok(msgs);
        }
        [Route("privados")]
        public IHttpActionResult GetPrivate([FromUri] string token, [FromUri] DateTime privdate)
        {
            if (string.IsNullOrWhiteSpace(token))
                return StatusCode(HttpStatusCode.Forbidden);
            var userid = TokenCommon.validarToken(token);
            if (userid <= 0)
                return Unauthorized();
            var email = TokenCommon.ObtenerWebUser(userid).Email;
            return Ok(_dbContext.Message.Where(m => m.Type == MSGTYPE_PRIVATE && m.Date > privdate && m.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).Select(_asMsgDto));
        }
        #region Admin
        [Route("admin/public")]
        public IHttpActionResult GetPublicAdmin([FromUri] string token, [FromUri] DateTime from, [FromUri] DateTime to)
        {
            var userid = TokenCommon.validarToken(token);
            if (userid <= 0 && userid != AUTH_USER_ID)
                return Unauthorized();
            var fromdate = from.Date;
            var todate = to.Date.AddDays(1);
            var result = _dbContext.Message.Where(m => m.Type == MSGTYPE_PUBLIC && m.Date >= fromdate && m.Date < todate).Select(_asMsgDto).ToList();
            return Ok(result);
        }
        [Route("admin/private")]
        public IHttpActionResult GetPrivateAdmin([FromUri] string token, [FromUri] DateTime from, [FromUri] DateTime to)
        {
            var userid = TokenCommon.validarToken(token);
            if (userid <= 0 && userid != AUTH_USER_ID)
                return Unauthorized();
            var fromdate = from.Date;
            var todate = to.Date.AddDays(1);
            var result = _dbContext.Message.Where(m => m.Type == MSGTYPE_PRIVATE && m.Date >= fromdate && m.Date < todate).Select(_asMsgDto).ToList();
            return Ok(result);
        }
        [Route("admin/privateuser")]
        public IHttpActionResult GetPrivateUserAdmin([FromUri] string token, [FromUri] DateTime from, [FromUri] DateTime to, [FromUri] string email)
        {
            var userid = TokenCommon.validarToken(token);
            if (userid <= 0 && userid != AUTH_USER_ID)
                return Unauthorized();
            var fromdate = from.Date;
            var todate = to.Date.AddDays(1);
            if (_dbContext.Usuarios.FirstOrDefault(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)) is Usuarios user)
            {
                var result = _dbContext.Message.Where(m => m.Type == MSGTYPE_PRIVATE && m.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) && m.Date >= fromdate && m.Date < todate).Select(_asMsgDto);
                return Ok(result);
            }
            return Ok(new List<MessageDto>());
        }
        [Route("admin/public")]
        [HttpPost]
        public IHttpActionResult PostPublicAdmin([FromUri] string token, [FromBody] Message message)
        {
            var userid = TokenCommon.validarToken(token);
            if (userid <= 0 && userid != AUTH_USER_ID)
                return Unauthorized();
            message.Date = DateTime.Now;
            message.Type = 1;
            var result = _dbContext.Message.Add(message);
            _dbContext.SaveChanges();
            return Ok(result);
        }
        [Route("admin/private")]
        [HttpPost]
        public IHttpActionResult PostPrivateAdmin([FromUri] string token, [FromBody] Message message)
        {
            var userid = TokenCommon.validarToken(token);
            if (userid <= 0 && userid != AUTH_USER_ID)
                return Unauthorized();
            if (_dbContext.Usuarios.FirstOrDefault(u => u.Email.Equals(message.Email, StringComparison.InvariantCultureIgnoreCase)) is Usuarios user)
            {
                message.Date = DateTime.Now;
                message.Type = 2;
                message.Email = user.Email.ToLower();
                var result = _dbContext.Message.Add(message);
                _dbContext.SaveChanges();
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }
        [Route("admin/message/{id:int}")]
        [HttpDelete]
        public IHttpActionResult DelMessageAdmin(int id, [FromUri] string token)
        {
            var userid = TokenCommon.validarToken(token);
            if (userid <= 0 && userid != AUTH_USER_ID)
                return Unauthorized();
            if (_dbContext.Message.FirstOrDefault(m => m.Id == id) is Message msg)
            {
                var result = _dbContext.Message.Remove(msg);
                _dbContext.SaveChanges();
                return Ok(result.Id);
            }
            return NotFound();
        }
        [Route("admin/users")]
        public IHttpActionResult GetUsersAdmin([FromUri] string token)
        {
            var userid = TokenCommon.validarToken(token);
            if (userid <= 0 && userid != AUTH_USER_ID)
                return Unauthorized();
            var users = _dbContext.Usuarios.Where(u => u.RazonSocial != string.Empty).Select(u => new { Name = u.RazonSocial, Email = u.Email.ToLower() });
            return Ok(users);
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
            base.Dispose(disposing);
        }

    }
}