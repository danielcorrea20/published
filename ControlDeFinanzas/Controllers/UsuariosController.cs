
using ControlDeFinanzas.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ControlDeFinanzas.Controllers
{
    public class UsuariosController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;

        //para generar las cookis uso SIGNINMANAGER 
        public UsuariosController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)

        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        //permito anonimo
        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]

        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);

            }
            var usuario = new Usuario() { Emial = modelo.Email };
            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

            //si va todo bien GRACIAS A IDENTITY LA CONTRASEÑA SE GUARDA EN LA BD ENCRIPTADA
            if (resultado.Succeeded)
            {
                //uso el sigin manager y ispersistent una opción nuestro usuario siga autenticado aunque cierre el navegador
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Transacciones");
            }
            else
            {
                //muestro los errores
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(modelo);
            }
        }
        //creo la acción de deslogeo
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Transacciones");

        }
        [AllowAnonymous]
        //creo la acción del login
        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            //no confio en la ata del usuario
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            //lockoutonfailure bloquea la aplicacion cuando has errado un número de veces en la contraseña. Lo desahbilito
            var resultado = await signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Transacciones");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "El nombre del usuario o contraseña es inconrrecto");
                return View(modelo);
            }
        }
    }
}