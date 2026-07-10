using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace TestLoginApp.Controllers
{
    public class LoginController : Controller
    {

        private readonly string _connectionString;

        public IActionResult Index()
        {
            return View();
        }

        public LoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string checkUserQuery = "SELECT intentos, timerFin FROM usuarios WHERE usuario = @Usuario";
                int failedAttempts = 0;
                DateTime? lockoutEnd = null;

                using (SqlCommand cmd = new SqlCommand(checkUserQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Usuario", username ?? "");
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            failedAttempts = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            lockoutEnd = reader.IsDBNull(1) ? (DateTime?)null : reader.GetDateTime(1);
                        }
                        else
                        {
                            TempData["AlertMessage"] = "Datos invalidos";
                            return RedirectToAction("LoginView");
                        }
                    }
                }

                if (failedAttempts >= 5 && lockoutEnd.HasValue)
                {
                    if (DateTime.Now < lockoutEnd.Value)
                    {
                        return RedirectToAction("Locked");
                    }
                    else
                    {
                        string resetLockoutQuery = "UPDATE usuarios SET intentos = 0, timerFin = NULL WHERE usuario = @Usuario";
                        using (SqlCommand resetCmd = new SqlCommand(resetLockoutQuery, conn))
                        {
                            resetCmd.Parameters.AddWithValue("@Usuario", username);
                            resetCmd.ExecuteNonQuery();
                        }
                        failedAttempts = 0;
                    }
                }
                string loginQuery = "SELECT COUNT(1) FROM usuarios WHERE usuario = @Usuario AND contraseña = @Password";
                using (SqlCommand cmd = new SqlCommand(loginQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Usuario", username ?? "");
                    cmd.Parameters.AddWithValue("@Password", password ?? "");

                    int credentialsValid = (int)cmd.ExecuteScalar();

                    if (credentialsValid == 1)
                    { 
                        string successQuery = "UPDATE usuarios SET intentos = 0, timerFin = NULL WHERE usuario = @Usuario";
                        using (SqlCommand successCmd = new SqlCommand(successQuery, conn))
                        {
                            successCmd.Parameters.AddWithValue("@Usuario", username);
                            successCmd.ExecuteNonQuery();
                        }
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        int newAttempts = failedAttempts + 1;

                        if (newAttempts >= 5)
                        {
                            string lockQuery = "UPDATE usuarios SET intentos = @Intentos, timerFin = @TimerFin WHERE usuario = @Usuario";
                            using (SqlCommand lockCmd = new SqlCommand(lockQuery, conn))
                            {
                                lockCmd.Parameters.AddWithValue("@Intentos", newAttempts);
                                lockCmd.Parameters.AddWithValue("@TimerFin", DateTime.Now.AddMinutes(15));
                                lockCmd.Parameters.AddWithValue("@Usuario", username);
                                lockCmd.ExecuteNonQuery();
                            }
                            return RedirectToAction("Locked");
                        }
                        else
                        {
                            string incQuery = "UPDATE usuarios SET intentos = @Intentos WHERE usuario = @Usuario";
                            using (SqlCommand incCmd = new SqlCommand(incQuery, conn))
                            {
                                incCmd.Parameters.AddWithValue("@Intentos", newAttempts);
                                incCmd.Parameters.AddWithValue("@Usuario", username);
                                incCmd.ExecuteNonQuery();
                            }

                            TempData["AlertMessage"] = "Datos invalidos";
                            return RedirectToAction("LoginView");
                        }
                    }
                }
            }
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult LoginView()
        {
            return View();
        }

        public IActionResult Locked()
        {
            return View();
        }
    }
}
