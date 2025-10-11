using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Expenses.Queries;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Reports
{
  public record GetExpenseCategorySummaryRequest
  {
    [QueryParam]
    public DateTime StartDate { get; init; }

    [QueryParam]
    public DateTime EndDate { get; init; }

    [QueryParam]
    public string Currency { get; init; } = string.Empty;

    private List<string>? _tagIds = [];
    [QueryParam]
    public List<string>? TagIds
    {
      get => _tagIds;
      set
      {
        _tagIds = value?.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();
      }
    }

    private List<string>? _categoryIds = [];
    [QueryParam]
    public List<string>? CategoryIds
    {
      get => _categoryIds;
      set
      {
        _categoryIds = value?.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();
      }
    }
  }

  public record CategorySummaryDataPointResponse
  {
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public int Count { get; init; }
  }

  public record ExpenseCategorySummaryResponse
  {
    public string Currency { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public List<CategorySummaryDataPointResponse> DataPoints { get; init; } = [];
  }

  public class GetExpenseCategorySummaryEndpoint : Endpoint<GetExpenseCategorySummaryRequest, SingleResponse<ExpenseCategorySummaryResponse>>
  {
    private readonly IMediator _mediator;

    public GetExpenseCategorySummaryEndpoint(IMediator mediator)
    {
      _mediator = mediator;
    }

    public override void Configure()
    {
      Get("/api/reports/expenses/category-summary");
      AllowAnonymous();
      Description(d => d
          .WithSummary("Get expense report with category aggregation")
          .Produces<SingleResponse<ExpenseCategorySummaryResponse>>(200)
          .ProducesProblem(400)
          .WithTags("Reports"));
    }

    public override async Task HandleAsync(GetExpenseCategorySummaryRequest req, CancellationToken ct)
    {
      var query = new GetExpenseCategorySummaryQuery
      {
        StartDate = req.StartDate.ToUniversalTime(),
        EndDate = req.EndDate.ToUniversalTime(),
        Currency = req.Currency,
        TagIds = req.TagIds?.Select(int.Parse).ToList() ?? [],
        CategoryIds = req.CategoryIds?.Select(int.Parse).ToList() ?? []
      };

      var result = await _mediator.Send(query, ct);

      if (result.IsError)
      {
        await ProblemResult.Of(result.Errors, HttpContext).ExecuteAsync(HttpContext);
        return;
      }

      var response = new ExpenseCategorySummaryResponse
      {
        Currency = result.Value.Currency,
        StartDate = result.Value.StartDate,
        EndDate = result.Value.EndDate,
        DataPoints = result.Value.DataPoints.Select(dp => new CategorySummaryDataPointResponse
        {
          CategoryId = dp.CategoryId,
          CategoryName = dp.CategoryName,
          TotalAmount = dp.TotalAmount,
          Count = dp.Count
        }).ToList()
      };

      await SendOkAsync(SingleResponse.Of(response), ct);
    }
  }
}
