﻿using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using HiWorld.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiWorld.Services.Data
{
    public class MessagesService : IMessagesService
    {
        private readonly IDeletableEntityRepository<Message> messageRepository;

        public MessagesService(IDeletableEntityRepository<Message> messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public async Task<int> AddMessage(int groupId, int profileId, string text)
        {
            var message = new Message
            {
                Text = text,
                GroupId = groupId,
                ProfileId = profileId,
            };

            await this.messageRepository.AddAsync(message);
            await this.messageRepository.SaveChangesAsync();

            return message.Id;
        }

        public async Task DeleteMessage(int messageId)
        {
            var message = this.messageRepository.All().Where(x => x.Id == messageId).FirstOrDefault();

            if (message != null)
            {
                this.messageRepository.Delete(message);
                await this.messageRepository.SaveChangesAsync();
            }
        }

        public T GetById<T>(int id)
        {
            return this.messageRepository.AllAsNoTracking().Where(x => x.Id == id).To<T>().FirstOrDefault();
        }

        public int GetMessageGroupId(int id)
        {
            var message = this.messageRepository.AllAsNoTracking().Where(x => x.Id == id).FirstOrDefault();
            if (message != null)
            {
                return message.Id;
            }

            return 0;
        }

        public bool IsOwner(int messageId, int profileId)
        {
            return this.messageRepository.AllAsNoTracking().Any(x => x.Id == messageId && x.ProfileId == profileId);
        }
    }
}