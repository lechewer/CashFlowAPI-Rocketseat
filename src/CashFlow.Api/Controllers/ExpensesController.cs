using CashFlow.Application.UseCases.Expenses.Delete;
using CashFlow.Application.UseCases.Expenses.GetAll;
using CashFlow.Application.UseCases.Expenses.GetById;
using CashFlow.Application.UseCases.Expenses.Register;
using CashFlow.Application.UseCases.Expenses.Update;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExpensesController : ControllerBase
{
   

    [HttpGet]
    [ProducesResponseType(typeof(ResponseExpensesJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAllExpenses([FromServices] IGetAllExpenseUseCase useCase)
    {
        var response = await useCase.Execute();
        if (response.Expenses.Count != 0)
            return Ok(response);

        return NoContent();
    }

    [HttpGet]
    [Route("{id:long}")]
    [ProducesResponseType(typeof(ResponseExpensesJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] long id, 
        [FromServices] IGetExpenseByIdUseCase useCase)
    {
        var response = await useCase.Execute(id);
        return Ok(response);
    }
    
   /* [HttpGet]
    [Route("{title}")]
    [ProducesResponseType(typeof(ResponseExpensesJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByTitle(
        [FromRoute] string title, 
        [FromServices] IGetExpenseByTitleUseCase useCase)
    {
        var response = await useCase.Execute(title);
        return Ok(response);
    }*/
    
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisteredExpenseJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromServices] IRegisterExpenseUseCase useCase,
        [FromBody] RequestExpenseJson request)
    {
        var response = await useCase.Execute(request);

        return Created(string.Empty, response);
    }
    
    [HttpPut]
    [Route("{id}")]
    [ProducesResponseType(typeof(ResponseExpensesJson), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] long id, 
        [FromServices] IUpdateExpenseUseCase useCase,
        [FromBody] RequestExpenseJson request
    )
    {
        await useCase.Execute(id, request);
        
        return NoContent();
    }

    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(typeof(ResponseExpensesJson), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] long id, 
        [FromServices] IDeleteExpenseUseCase useCase)
    {
        await useCase.Execute(id);
        
        return NoContent();
    }
 
   
 
}

    