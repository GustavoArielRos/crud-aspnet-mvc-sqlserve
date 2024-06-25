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


        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);

            if(product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            //cria um productDto do produto
            var productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            //armazena os dados nessa "ViewData"
            //ela serve para exibir os dados na View(página) de uma forma simples
            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreateAt.ToString("MM/dd/yyyy");


            return View(productDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            //Adicionando o produto encontrado no banco de dados usando a ID
            var product = context.Products.Find(id);

            //se não tem produto vai para a página principal com a lista dos produtos
            if(product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            //se tem algo no modal que não é válido vai retorna pra página edit de volta
            if(!ModelState.IsValid)
            {   
                //eu coloco isso pq o productDto não tem esses atributos para trabalhar
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAT"] = product.CreateAt.ToString("MM/dd/yyyy");

                return View(productDto);//retorno a página Edit
            }

            //atualizando o arquivo de imagem caso tenha um novo arquivo de imagem
            string newFileName = product.ImageFileName;//cria um novo arquivo com as inf da imagem atual
            //se tem uma nova imagem
            if(productDto.ImageFile != null)
            {   
                //gera um novo nome de arquivo baseado na data atual
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //adiciona o arquivo original ao novo nome de arquivo criado acima(essa extensão tem a imagem)
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                //Código para salvar a nova imagem no sistema
                //cria uma string(define o caminho) usando a raiz do servidor + subpasta + a variável que tem o novo arquivo
                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;

                //crinado o fluxo no sistema para poder escrever nesse caminho
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);//copia o conteúdo da imagem enviada para o fluxo
                }


                //deletando a imagem antiga
                //constroi um caminho completo para a imagem antiga,combinando o caminho raiz do servidor + subdiretório + nome do arquivo
                string oldImageFullPath = environment.WebRootPath + "/products" + product.ImageFileName;
                //deleta o arquivo de imagem antigo do sistema de arquivos
                System.IO.File.Delete(oldImageFullPath);
            }

            //atualizando o produto na base de dados
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;

            context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        public IActionResult Delete(int id)
        {   
            //vai pegar o produto a partir da ID e jogar nessa variável
            var product = context.Products.Find(id);

            //caso não exista o produto volta para página inicial
            if(product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            //faz o caminho pegando a raiz do servidor + subpasta + nome do arquivo
            string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            //delentando a imagem no sistema
            System.IO.File.Delete(imageFullPath);

            //removendo os outros atributos do produto
            context.Products.Remove(product);
            //salvando as mudanças
            context.SaveChanges();

            return RedirectToAction("Index", "Products");

        }


    }
}
