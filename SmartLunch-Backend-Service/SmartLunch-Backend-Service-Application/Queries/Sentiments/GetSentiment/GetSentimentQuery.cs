using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Sentiments;

namespace SmartLunch.Backend.Service.Application.Queries.Sentiments.GetSentiment;

public class GetSentimentQuery : IRequest<GetSentimentResponse>
{
    public int SentimentId { get; set; }

    public GetSentimentQuery(int sentimentId)
    {
        SentimentId = sentimentId;
    }
}
