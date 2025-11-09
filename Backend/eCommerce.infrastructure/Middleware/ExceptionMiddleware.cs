using System;
using System.Net;
using System.Threading.Tasks;
using eCommerce.Application.Services.Interfaces.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Infrastructure.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var logger = context.RequestServices.GetRequiredService<IAppLogger<ExceptionMiddleware>>();

            try
            {
                await _next(context);
            }
            catch (SqlException sqlEx)
            {
                logger.LogError("SQL Exception occurred.", sqlEx);
                await HandleSqlExceptionAsync(context, sqlEx);
            }
            catch (DbUpdateException dbEx)
            {
                logger.LogError("Entity Framework Core Update Exception occurred.", dbEx);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("A database update error occurred. Please verify your data and try again.");
            }
            catch (ArgumentNullException argEx)
            {
                logger.LogError("Argument Null Exception occurred.", argEx);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("A required argument was null.");
            }
            catch (InvalidOperationException invalidOpEx)
            {
                logger.LogError("Invalid Operation Exception occurred.", invalidOpEx);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("An invalid operation was attempted.");
            }
            catch (Exception ex)
            {
                logger.LogError("An unhandled exception occurred.", ex);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("An unexpected server error occurred. Please try again later.");
            }
        }

        private static async Task HandleSqlExceptionAsync(HttpContext context, SqlException sqlEx)
        {
            context.Response.ContentType = "application/json";

            switch (sqlEx.Number)
            {
                case 2627: // Unique constraint violation
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    await context.Response.WriteAsync("A unique constraint violation occurred.");
                    break;

                case 515: // Cannot insert null
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync("Cannot insert null value into required field.");
                    break;

                case 547: // Foreign key constraint violation
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    await context.Response.WriteAsync("Foreign key constraint violation occurred.");
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync("A SQL Server error occurred.");
                    break;
            }
        }
    }
}
