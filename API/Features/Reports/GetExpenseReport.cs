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
    public record GetExpenseReportRequest
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

    public record ExpenseReportResponse
    {
        public string Currency { get; init; } = string.Empty;
        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; init; }
        public List<ExpenseReportDataPointResponse> DataPoints { get; init; } = [];
    }

    public record ExpenseReportDataPointResponse
    {
        public DateOnly Date { get; init; }
        public decimal Amount { get; init; }
    }

    public class GetExpenseReportEndpoint : Endpoint<GetExpenseReportRequest, SingleResponse<ExpenseReportResponse>>
    {
        private readonly IMediator _mediator;

        public GetExpenseReportEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("/api/reports/expenses");
            AllowAnonymous();
            Description(d => d
                .WithSummary("Get expense report with daily aggregation")
                .Produces<SingleResponse<ExpenseReportResponse>>(200)
                .ProducesProblem(400)
                .WithTags("Reports"));
        }

        public override async Task HandleAsync(GetExpenseReportRequest req, CancellationToken ct)
        {
            var query = new GetExpenseReportQuery
            {
                StartDate = DateOnly.FromDateTime(req.StartDate),
                EndDate = DateOnly.FromDateTime(req.EndDate),
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

            var response = new ExpenseReportResponse
            {
                Currency = result.Value.Currency,
                StartDate = result.Value.StartDate,
                EndDate = result.Value.EndDate,
                DataPoints = result.Value.DataPoints.Select(dp => new ExpenseReportDataPointResponse
                {
                    Date = dp.Date,
                    Amount = dp.Amount
                }).ToList()
            };

            await SendOkAsync(SingleResponse.Of(response), ct);
        }
    }
}