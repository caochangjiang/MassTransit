﻿// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;


    public class HaModel :
        IHaModel
    {
        readonly ConnectionContext _connectionContext;
        readonly IModel _model;
        readonly object _publishLock = new object();
        readonly object _rpcLock = new object();
        readonly ConcurrentDictionary<ulong, PendingPublish> _published;

        public HaModel(ConnectionContext connectionContext, IModel model)
        {
            _published = new ConcurrentDictionary<ulong, PendingPublish>();
            _connectionContext = connectionContext;
            _model = model;
            _model.ConfirmSelect();
            _model.BasicAcks += ModelOnBasicAcks;
            _model.BasicNacks += ModelOnBasicNacks;
            _model.BasicReturn += ModelOnBasicReturn;
        }

        public void ExchangeBindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            lock(_rpcLock)
            _model.ExchangeBindNoWait(destination, source, routingKey, arguments);
        }

        public void ExchangeUnbindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            lock (_rpcLock)
                _model.ExchangeUnbindNoWait(destination, source, routingKey, arguments);
        }

        public void QueueDeclareNoWait(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            lock (_rpcLock)
                _model.QueueDeclareNoWait(queue, durable, exclusive, autoDelete, arguments);
        }

        public void QueueBindNoWait(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            lock (_rpcLock)
                _model.QueueBindNoWait(queue, exchange, routingKey, arguments);
        }

        public void QueueDeleteNoWait(string queue, bool ifUnused, bool ifEmpty)
        {
            lock (_rpcLock)
                _model.QueueDeleteNoWait(queue, ifUnused, ifEmpty);
        }

        public void ExchangeDeleteNoWait(string exchange, bool ifUnused)
        {
            lock (_rpcLock)
                _model.ExchangeDeleteNoWait(exchange, ifUnused);
        }

        public void ExchangeDeclareNoWait(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            lock (_rpcLock)
                _model.ExchangeDeclareNoWait(exchange, type, durable, autoDelete, arguments);
        }

        public void Dispose()
        {
            lock (_rpcLock)
                _model.Dispose();
        }

        public IBasicProperties CreateBasicProperties()
        {
            return _model.CreateBasicProperties();
        }

        public void ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            lock (_rpcLock)
                _model.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);
        }

        public void ExchangeDeclare(string exchange, string type, bool durable)
        {
            lock (_rpcLock)
                _model.ExchangeDeclare(exchange, type, durable);
        }

        public void ExchangeDeclare(string exchange, string type)
        {
            lock (_rpcLock)
                _model.ExchangeDeclare(exchange, type);
        }

        public void ExchangeDeclarePassive(string exchange)
        {
            lock (_rpcLock)
                _model.ExchangeDeclarePassive(exchange);
        }

        public void ExchangeDelete(string exchange, bool ifUnused)
        {
            lock (_rpcLock)
                _model.ExchangeDelete(exchange, ifUnused);
        }

        public void ExchangeDelete(string exchange)
        {
            lock (_rpcLock)
                _model.ExchangeDelete(exchange);
        }

        public void ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            lock (_rpcLock)
                _model.ExchangeBind(destination, source, routingKey, arguments);
        }

        public void ExchangeBind(string destination, string source, string routingKey)
        {
            lock (_rpcLock)
                _model.ExchangeBind(destination, source, routingKey);
        }

        public void ExchangeUnbind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            lock (_rpcLock)
                _model.ExchangeUnbind(destination, source, routingKey, arguments);
        }

        public void ExchangeUnbind(string destination, string source, string routingKey)
        {
            lock (_rpcLock)
                _model.ExchangeUnbind(destination, source, routingKey);
        }

        public QueueDeclareOk QueueDeclare()
        {
            lock (_rpcLock)
                return _model.QueueDeclare();
        }

        public QueueDeclareOk QueueDeclarePassive(string queue)
        {
            lock (_rpcLock)
                return _model.QueueDeclarePassive(queue);
        }

        public QueueDeclareOk QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete,
            IDictionary<string, object> arguments)
        {
            lock (_rpcLock)
                return _model.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
        }

        public void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            lock (_rpcLock)
                _model.QueueBind(queue, exchange, routingKey, arguments);
        }

        public void QueueBind(string queue, string exchange, string routingKey)
        {
            lock (_rpcLock)
                _model.QueueBind(queue, exchange, routingKey);
        }

        public void QueueUnbind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            lock (_rpcLock)
                _model.QueueUnbind(queue, exchange, routingKey, arguments);
        }

        public uint QueuePurge(string queue)
        {
            lock (_rpcLock)
                return _model.QueuePurge(queue);
        }

        public uint QueueDelete(string queue, bool ifUnused, bool ifEmpty)
        {
            lock (_rpcLock)
                return _model.QueueDelete(queue, ifUnused, ifEmpty);
        }

        public uint QueueDelete(string queue)
        {
            lock (_rpcLock)
                return _model.QueueDelete(queue);
        }

        public void ConfirmSelect()
        {
            lock (_rpcLock)
                _model.ConfirmSelect();
        }

        public bool WaitForConfirms()
        {
            return _model.WaitForConfirms();
        }

        public bool WaitForConfirms(TimeSpan timeout)
        {
            return _model.WaitForConfirms(timeout);
        }

        public bool WaitForConfirms(TimeSpan timeout, out bool timedOut)
        {
            return _model.WaitForConfirms(timeout, out timedOut);
        }

        public void WaitForConfirmsOrDie()
        {
            _model.WaitForConfirmsOrDie();
        }

        public void WaitForConfirmsOrDie(TimeSpan timeout)
        {
            _model.WaitForConfirmsOrDie(timeout);
        }

        public int ChannelNumber
        {
            get { return _model.ChannelNumber; }
        }

        public string BasicConsume(string queue, bool noAck, IBasicConsumer consumer)
        {
            lock (_rpcLock)
                return _model.BasicConsume(queue, noAck, consumer);
        }

        public string BasicConsume(string queue, bool noAck, string consumerTag, IBasicConsumer consumer)
        {
            lock (_rpcLock)
                return _model.BasicConsume(queue, noAck, consumerTag, consumer);
        }

        public string BasicConsume(string queue, bool noAck, string consumerTag, IDictionary<string, object> arguments,
            IBasicConsumer consumer)
        {
            lock (_rpcLock)
                return _model.BasicConsume(queue, noAck, consumerTag, arguments, consumer);
        }

        public string BasicConsume(string queue, bool noAck, string consumerTag, bool noLocal, bool exclusive,
            IDictionary<string, object> arguments,
            IBasicConsumer consumer)
        {
            lock (_rpcLock)
                return _model.BasicConsume(queue, noAck, consumerTag, noLocal, exclusive, arguments, consumer);
        }

        public void BasicCancel(string consumerTag)
        {
            lock (_rpcLock)
                _model.BasicCancel(consumerTag);
        }

        public void BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            lock (_rpcLock)
                _model.BasicQos(prefetchSize, prefetchCount, global);
        }

        public void BasicPublish(PublicationAddress addr, IBasicProperties basicProperties, byte[] body)
        {
            BasicPublish(addr.ExchangeName, addr.RoutingKey, basicProperties, body);
        }

        public void BasicPublish(string exchange, string routingKey, IBasicProperties basicProperties, byte[] body)
        {
            BasicPublish(exchange, routingKey, false, basicProperties, body);
        }

        public void BasicPublish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body)
        {
            BasicPublish(exchange, routingKey, mandatory, false, basicProperties, body);
        }

        public void BasicPublish(string exchange, string routingKey, bool mandatory, bool immediate, IBasicProperties basicProperties,
            byte[] body)
        {
            lock (_publishLock)
                _model.BasicPublish(exchange, routingKey, mandatory, immediate, basicProperties, body);
        }

        public Task BasicPublishAsync(PublicationAddress addr, IBasicProperties basicProperties, byte[] body)
        {
            return BasicPublishAsync(addr.ExchangeName, addr.RoutingKey, basicProperties, body);
        }

        public Task BasicPublishAsync(string exchange, string routingKey, IBasicProperties basicProperties, byte[] body)
        {
            return BasicPublishAsync(exchange, routingKey, false, basicProperties, body);
        }

        public Task BasicPublishAsync(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body)
        {
            return BasicPublishAsync(exchange, routingKey, mandatory, false, basicProperties, body);
        }

        public Task BasicPublishAsync(string exchange, string routingKey, bool mandatory, bool immediate, IBasicProperties basicProperties,
            byte[] body)
        {
            PendingPublish pendingPublish;
            lock (_publishLock)
            {
                ulong publishTag = _model.NextPublishSeqNo;
                BasicPublish(exchange, routingKey, mandatory, immediate, basicProperties, body);

                pendingPublish = new PendingPublish(_connectionContext, exchange, publishTag);
                _published.TryAdd(publishTag, pendingPublish);
            }

            return pendingPublish.Task;
        }

        public void BasicAck(ulong deliveryTag, bool multiple)
        {
            lock (_rpcLock)
                _model.BasicAck(deliveryTag, multiple);
        }

        public void BasicReject(ulong deliveryTag, bool requeue)
        {
            lock (_rpcLock)
                _model.BasicReject(deliveryTag, requeue);
        }

        public void BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            lock (_rpcLock)
                _model.BasicNack(deliveryTag, multiple, requeue);
        }

        public void BasicRecover(bool requeue)
        {
            lock (_rpcLock)
                _model.BasicRecover(requeue);
        }

        public void BasicRecoverAsync(bool requeue)
        {
            lock (_rpcLock)
                _model.BasicRecoverAsync(requeue);
        }

        public BasicGetResult BasicGet(string queue, bool noAck)
        {
            return _model.BasicGet(queue, noAck);
        }

        public void TxSelect()
        {
            lock (_rpcLock)
                _model.TxSelect();
        }

        public void TxCommit()
        {
            lock (_rpcLock)
                _model.TxCommit();
        }

        public void TxRollback()
        {
            lock (_rpcLock)
                _model.TxRollback();
        }

        public void Close()
        {
            lock (_rpcLock)
                _model.Close();
        }

        public void Close(ushort replyCode, string replyText)
        {
            lock (_rpcLock)
                _model.Close(replyCode, replyText);
        }

        public void Abort()
        {
            lock (_rpcLock)
                _model.Abort();
        }

        public void Abort(ushort replyCode, string replyText)
        {
            lock (_rpcLock)
                _model.Abort(replyCode, replyText);
        }

        public IBasicConsumer DefaultConsumer
        {
            get { return _model.DefaultConsumer; }
            set { _model.DefaultConsumer = value; }
        }

        public ShutdownEventArgs CloseReason
        {
            get { return _model.CloseReason; }
        }

        public bool IsOpen
        {
            get { return _model.IsOpen; }
        }

        public bool IsClosed
        {
            get { return _model.IsClosed; }
        }

        public ulong NextPublishSeqNo
        {
            get { return _model.NextPublishSeqNo; }
        }

        public event EventHandler<ShutdownEventArgs> ModelShutdown
        {
            add { _model.ModelShutdown += value; }
            remove { _model.ModelShutdown -= value; }
        }

        public event EventHandler<BasicReturnEventArgs> BasicReturn
        {
            add { _model.BasicReturn += value; }
            remove { _model.BasicReturn -= value; }
        }

        public event EventHandler<BasicAckEventArgs> BasicAcks
        {
            add { _model.BasicAcks += value; }
            remove { _model.BasicAcks -= value; }
        }

        public event EventHandler<BasicNackEventArgs> BasicNacks
        {
            add { _model.BasicNacks += value; }
            remove { _model.BasicNacks -= value; }
        }

        public event EventHandler<CallbackExceptionEventArgs> CallbackException
        {
            add { _model.CallbackException += value; }
            remove { _model.CallbackException -= value; }
        }

        public event EventHandler<FlowControlEventArgs> FlowControl
        {
            add { _model.FlowControl += value; }
            remove { _model.FlowControl -= value; }
        }

        public event EventHandler<EventArgs> BasicRecoverOk
        {
            add { _model.BasicRecoverOk += value; }
            remove { _model.BasicRecoverOk -= value; }
        }

        void ModelOnBasicReturn(object model, BasicReturnEventArgs args)
        {
        }

        void ModelOnBasicNacks(object model, BasicNackEventArgs args)
        {
            if (args.Multiple)
            {
                ulong[] ids = _published.Keys.Where(x => x <= args.DeliveryTag).ToArray();
                foreach (ulong id in ids)
                {
                    PendingPublish value;
                    if (_published.TryRemove(id, out value))
                        value.Nack();
                }
            }
            else
            {
                PendingPublish value;
                if (_published.TryRemove(args.DeliveryTag, out value))
                    value.Nack();
            }
        }

        void ModelOnBasicAcks(object model, BasicAckEventArgs args)
        {
            if (args.Multiple)
            {
                ulong[] ids = _published.Keys.Where(x => x <= args.DeliveryTag).ToArray();
                foreach (ulong id in ids)
                {
                    PendingPublish value;
                    if (_published.TryRemove(id, out value))
                        value.Ack();
                }
            }
            else
            {
                PendingPublish value;
                if (_published.TryRemove(args.DeliveryTag, out value))
                    value.Ack();
            }
        }
    }
}