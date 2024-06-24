using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BestStoreMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        // Campo para acessar informações sobre o ambiente de hospedagem
        private readonly IWebHostEnvironment environment;
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            var products = context.Products.ToList();//pega todas as informações da tabela Products

            return View(products);//vai abrir a página Index com essas informações para serem usadas
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]//método de criação
        public IActionResult Create(ProductDto productDto)
        {
            // Verifica se a propriedade ImageFile do DTO do produto é nula.
            if (productDto.ImageFile == null)
            {
                // Adiciona um erro ao ModelState se a imagem não for fornecida.
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            // Verifica se o estado do modelo é válido, ou seja, se não há erros de validação.
            if (!ModelState.IsValid)
            {
                // Se o modelo não for válido, retorna a view com o DTO do produto
                // para que o usuário possa corrigir os erros.
                return View(productDto);
            }

            //SALVANDO O ARQUIVO DE IMAGEM
            //gera um novo nome de arquivo único baseado na data e hora atuais
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            //adiciona a extensão do arquivo original ao novo nome do arquivo
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);
            //define o caminho completo onde a imagem será salva, combinando o diretório raiz da aplicação e o novo nome do arquivo
            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            //Cria um novo arquivo no caminho especificado
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);//copia o conteudo enviado do usário para o novo arquivo criado
            }

            //SALVANDO O PRODUTO NA BASE DE DADOS
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreateAt = DateTime.Now,
            };

            context.Products.Add(product);
            context.SaveChanges();


            // Se o estado do modelo for válido e a imagem estiver presente,
            // redireciona para a ação Index no controlador Products.
            return RedirectToAction("Index", "Products");
        }

    }
}
