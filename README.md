# Aprendizados:

- o contexto(DbContext) possui e controla todas as informações importantes no banco de dados
- quando o programa precisa saber algo, ele "pergunta" para o contexto que então olha para o banco de dados
- quando algo novo é adicionado o contexto atualiza essa informação no banco de dados
- o programa sempre irá consultar o contexto para saber algo do banco de dados

-MaxLength --> define o máximo de caracteres que o atributo pode ter

-[Precision(16,2)]//quantidade máxima de números antes da vírgula e depois da vírgula

- eu posso ter 2 métodos com o mesmo nome se eles tiverem assinaturas diferentes

-IWebHostEnvironment --> Permite acessar o caminho físico do diretório raiz da aplicação web(usado para salver ou ler arquivos no servidor)
			 em uma parte do código usa "environment.WebRootPath" utilizado para obter o caminho onde as imagens
			 dos produtos serão salvas


-//armazena os dados nessa "ViewData"
 //ela serve para exibir os dados na View(página) de uma forma simples	


-cada view que ele cria , ele tem um método para a página e outra para o tipo de ação que a página faz
- se for criar(visualização e post)
- se for editar(edição e put)
- é realmente isso, cria uma que é a pagina e outra que é o método que ela faz
- curiosamente o delete não tem

- para imagem se queremos, adicionar, modificar ou deletar, sempre temos que criar e trabalhar com o caminho do arquivo

- quando criar os botões que acionam métodos da controler sempre usar o "asp-route-id" para passar o produto
