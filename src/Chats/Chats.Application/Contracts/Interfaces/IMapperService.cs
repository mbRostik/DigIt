﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.Contracts.Interfaces
{
    public interface IMapperService
    {
        IMapper Mapper_Message_To_GiveUserChatMessagesDTO();
    }
}
