using System;

namespace Tippspiel.Contracts.Models
{
    public class GroupInfoModel
    {
        public int Id { get; private set; }
        public string Text { get; private set; }

        public GroupInfoModel(int id, string text)
        {
            Id = id;
            Text = text;
        }
    }
}
