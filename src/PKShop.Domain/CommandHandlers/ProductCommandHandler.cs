using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PKShop.Core.Bus;
using PKShop.Core.Notifications;
using PKShop.Domain.Commands.Products;
using PKShop.Domain.DomainClasses.Products;
using PKShop.Domain.Events.Products;
using PKShop.Domain.Interfaces;

namespace PKShop.Domain.CommandHandlers
{
    public class ProductCommandHandler : CommandHandler,
        IRequestHandler<CreateNewProductCommand>,
        IRequestHandler<UpdateProductCommand>,
        IRequestHandler<DeleteProductCommand>
        {
            private readonly IProductRepository _productRepository;
            private readonly IMediatorHandler _bus;

            public ProductCommandHandler(IProductRepository productRepository, IMediatorHandler bus,
                IUnitOfWork uow, INotificationHandler<DomainNotification> notification) :
                base(uow, bus, notification)
            {
                _productRepository = productRepository;
                _bus = bus;
            }

            public async Task<Unit> Handle(CreateNewProductCommand command, CancellationToken cancellationToken)
            {
                var product = new Product(Guid.NewGuid(), command.Name, command.Quantity, command.Cost);
                var productFromDb = await _productRepository.GetAsync(product.Id);

                if (productFromDb != null)
                {
                    await _bus.RaiseEvent(new DomainNotification(command.MessageType, "Product with this name already exists."));
                    return Unit.Value;
                }
                await _productRepository.CreateAsync(product);

                if(await CommitAsync())
                {
                    await _bus.RaiseEvent(new ProductCreatedEvent(product.Id, product.Name, product.Quantity, product.Cost));
                }

                return Unit.Value;
        }

            public async Task<Unit> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
            {
                var productToEdit = await _productRepository.GetAsync(x => x.Name == command.Name);              

                _productRepository.Update(productToEdit);

                if (await CommitAsync())
                {
                   await _bus.RaiseEvent(new ProductUpdatedEvent(productToEdit.Id, productToEdit.Name, productToEdit.Quantity, productToEdit.Cost));
                }

                return Unit.Value;
            }

            public async Task<Unit> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
            {
                _productRepository.Delete(command.Id);

                if (await CommitAsync())
                {
                    await _bus.RaiseEvent(new ProductDeletedEvent(command.Id));
                }

                return Unit.Value;
            }
        }
}