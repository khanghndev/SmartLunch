using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationChatbot;
using SmartLunch.Backend.Service.Application.OrganizationChatbot;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationChatbot.SendOrganizationChatbotMessage;

public sealed class SendOrganizationChatbotMessageCommandHandler
    : IRequestHandler<SendOrganizationChatbotMessageCommand, OrganizationChatbotMessageResponse>
{
    private readonly IOrganizationChatbotService _chatbot;

    public SendOrganizationChatbotMessageCommandHandler(IOrganizationChatbotService chatbot)
    {
        _chatbot = chatbot;
    }

    public Task<OrganizationChatbotMessageResponse> Handle(
        SendOrganizationChatbotMessageCommand request,
        CancellationToken cancellationToken) =>
        _chatbot.ProcessAsync(request.UserId, request.Message, cancellationToken);
}
