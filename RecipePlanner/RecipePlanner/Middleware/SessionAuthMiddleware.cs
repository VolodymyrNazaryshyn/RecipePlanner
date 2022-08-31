using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace RecipePlanner.Middleware
{
	public class SessionAuthMiddleware
	{
        // обязательное поле для Middleware (ссылка на следующий слой)
        private readonly RequestDelegate _next;

        // обязательная форма конструктора
        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // обязательный метод класса
        public async Task InvokeAsync(HttpContext context, Services.IAuthService authService, UserdbContext userdbContext)
        {
            String userId = context.Session.GetString("userId");
            if(userId != null)
            {
                authService.Set(userId);
                // Извлекаем из сессии метку времени начала авторизации и вычисляем длительность (авторизованного сеанса)
                long authMoment = Convert.ToInt64(context.Session.GetString("AuthMoment"));
                long authDuration = (DateTime.Now.Ticks - authMoment) / (long)1e7; // узнаем длительность нашей сессии
                if(authDuration > 100) // Предельная длительность сеанса авторизации
                {
                    // Стираем из сессии признак авторизации
                    context.Session.Remove("userId");
                    context.Session.Remove("AuthMoment");
                    // По правилам безопасности: если меняется состояние авторизации то необходимо перезагрузить систему (страницу)
                    context.Response.Redirect("/");

                    userdbContext.SaveChanges();
                    return;
                }
            }

            await _next(context);
        }
    }
}
