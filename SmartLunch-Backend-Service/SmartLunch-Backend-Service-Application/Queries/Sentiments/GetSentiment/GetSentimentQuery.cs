using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Sentiments;

namespace SmartLunch.Backend.Service.Application.Queries.Sentiments.GetSentiment;

public class GetSentimentQuery : IRequest<GetSentimentResponse>
{
    public Guid SentimentId { get; set; }

    public GetSentimentQuery(Guid sentimentId)
    {
        SentimentId = sentimentId;
    }
}
