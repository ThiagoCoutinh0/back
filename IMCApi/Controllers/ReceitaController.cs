using Microsoft.AspNetCore.Mvc;

namespace IMCApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReceitaController : ControllerBase
    {
        private static List<Receita> receitas = new List<Receita>
        {
            new Receita { ReceitaId = 1, NomeReceita = "Salada de Frango", Tipo = "Fit", Ingredientes = "Frango e Alface" },
            new Receita { ReceitaId = 2, NomeReceita = "Bolo de Chocolate", Tipo = "Normal", Ingredientes = "Farinha e Chocolate" },
            new Receita { ReceitaId = 3, NomeReceita = "Smoothie Verde", Tipo = "Fit", Ingredientes = "Couve e Limão" },
            new Receita { ReceitaId = 4, NomeReceita = "Pizza Calabresa", Tipo = "Normal", Ingredientes = "Molho de tomate e Calabresa" },
        };
        private readonly ILogger<ReceitaController> _logger;

        public ReceitaController(ILogger<ReceitaController> logger)
        {
            _logger = logger;
        }

        // Endpoint para sugerir receita com base no IMC
        [HttpGet("Sugerir/{imc}", Name = "GetReceitaSugerida")]
        public IActionResult GetReceitaSugerida(double imc)
        {
            Random rand = new Random();
            if (imc < 25)
            {
                var receitasNormal = receitas.Where(r => r.Tipo == "Normal").ToList();
                Receita receitaAleatoria = receitasNormal[rand.Next(receitasNormal.Count)];
                return new JsonResult(receitaAleatoria);
            }
            else
            {
                var receitasFit = receitas.Where(r => r.Tipo == "Fit").ToList();
                Receita receitaAleatoria = receitasFit[rand.Next(receitasFit.Count)];
                return new JsonResult(receitaAleatoria);
            }
        }

        // Endpoint para buscar uma receita específica pelo ID para edição
        [HttpGet("Editar/{id}", Name = "GetReceitaEditar")]
        public IActionResult GetReceitaEditar(int id)
        {
            var receita = receitas.FirstOrDefault(w => w.ReceitaId == id);
            if (receita == null)
            {
                return NotFound("Receita não encontrada.");
            }
            return new JsonResult(receita);
        }

        [HttpGet]
        public IActionResult ListarTodas()
        {
            return Ok(receitas);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Receita novaReceita)
        {
            if (novaReceita == null || string.IsNullOrEmpty(novaReceita.NomeReceita))
            {
                return BadRequest("Dados inválidos.");
            }

            // Define um novo ReceitaId baseado no maior ID atual
            int novoId = receitas.Max(r => r.ReceitaId) + 1;
            novaReceita.ReceitaId = novoId;
            receitas.Add(novaReceita);

            return Ok(novaReceita);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Receita receitaAtualizada)
        {
            if (receitaAtualizada == null)
            {
                return BadRequest("Dados inválidos.");
            }

            var receitaExistente = receitas.FirstOrDefault(r => r.ReceitaId == id);
            if (receitaExistente == null)
            {
                return NotFound("Receita não encontrada.");
            }

            // Atualiza os dados da receita existente
            receitaExistente.NomeReceita = receitaAtualizada.NomeReceita;
            receitaExistente.Tipo = receitaAtualizada.Tipo;
            receitaExistente.Ingredientes = receitaAtualizada.Ingredientes;

            return Ok(receitaExistente);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var receita = receitas.FirstOrDefault(r => r.ReceitaId == id);
            if (receita == null)
            {
                return NotFound("Receita não encontrada.");
            }

            receitas.Remove(receita);
            return NoContent();  // Retorna 204 se a remoção for bem-sucedida
        }
    }
}