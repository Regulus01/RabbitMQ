using AutoMapper;
using Consumer.Infra.Context;
using Consumer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Consumer.Controllers;

[Controller]
[Route("api/Products")]
public class ProductController : ControllerBase
{
    private MyDbContext _dbContext;
    private IMapper _mapper;

    public ProductController(MyDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Obtém todos os produtos do sistema
    /// </summary>
    /// <returns>Produtos do sistema</returns>
    [HttpGet]
    public IActionResult GetProducts()
    {
        var products = _dbContext.Products.OrderByDescending(x => x.Name)
                                          .ToList();
        
        if (products.Count == 0)
            return BadRequest("Produtos não encontrados");

        var productsViewModel = _mapper.Map<List<GetProductOrderViewModel>>(products);
        
        return Ok(productsViewModel);
    }
}