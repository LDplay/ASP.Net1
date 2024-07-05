using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Models.Home
{
    public class SignUpFormModel
    {
        [FromForm(Name = "user-email")]
        public string UserEmail { get; set; } = null!;

        [FromForm(Name = "user-name")]
        public string UserName { get; set; } = null!;
    }
}
/* Моделі (в ASP) - це класи, за допомогою яких реалізується передача 
 * комплексних даних (набору даних). В інших системах для цього вживають
 * термін DTO (Data Transfer Object)
 * 
 *  Розрізняють моделі форм (FormModel) та моделі представленб (PageModel)
 */
