﻿using PKShop.Core.Exceptions;
using PKShop.Core.Interfaces;
using System;

namespace PKShop.Core.Domain.Products
{
    public class Category : IAggregateRoot
    {
        public Guid Id { get; set; }
        public string Name { get; protected set; }

        protected Category()
        {
        }

        public Category(Guid id, string name)
        {
            Id = id;
            SetName(name);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new PKShopException(ErrorCodes.InvalidProductCategory,
                    "CategoryName can not be empty.");
            }
            if (name.Length > 100)
            {
                throw new PKShopException(ErrorCodes.InvalidProductCategory,
                    "CategoryName can not be longer than 100 characters.");
            }
            Name = name;
        }

        public static Category Create(string name)
            => Create(Guid.NewGuid(), name);

        public static Category Create(Guid id, string name)
            => new Category(id, name);
    }
}
