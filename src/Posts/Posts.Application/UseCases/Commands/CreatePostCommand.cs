﻿using MediatR;
using Posts.Application.Contracts.DTOs;
using Posts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.UseCases.Commands
{
    public record CreatePostCommand(CreatePostDTO model) : IRequest<bool>;
}
