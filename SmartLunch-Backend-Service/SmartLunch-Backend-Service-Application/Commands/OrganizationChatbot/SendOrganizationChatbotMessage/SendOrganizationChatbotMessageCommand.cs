using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationChatbot;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationChatbot.SendOrganizationChatbotMessage;

public sealed record SendOrganizationChatbotMessageCommand(int UserId, string Message)
    : IRequest<OrganizationChatbotMessageResponse>;
