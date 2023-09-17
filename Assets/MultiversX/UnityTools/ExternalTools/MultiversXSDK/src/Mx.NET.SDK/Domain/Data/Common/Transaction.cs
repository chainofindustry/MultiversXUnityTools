using System;
using System.Linq;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.NET.SDK.Domain.Helper;

namespace Mx.NET.SDK.Domain.Data.Common
{
    public class Action
    {
        public string Category { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Argument Arguments { get; private set; }

        private Action() { }

        public static Action From(ActionDto action)
        {
            if (action == null) return null;

            return new Action()
            {
                Category = action.Category,
                Name = action.Name,
                Description = action.Description,
                Arguments = Argument.From(action.Arguments)
            };
        }
    }

    public class Argument
    {
        public Transfer[] Transfers { get; private set; }
        public Address Receiver { get; private set; }
        public string FunctionName { get; private set; }
        public string[] FunctionArgs { get; private set; }
        public Assets ReceiverAssets { get; private set; }

        private Argument() { }

        public static Argument From(ArgumentDto argument)
        {
            if (argument == null) return null;

            return new Argument()
            {
                Transfers = Transfer.From(argument.Transfers),
                Receiver = Address.FromBech32(argument.Receiver),
                FunctionName = argument.FunctionName,
                FunctionArgs = argument.FunctionArgs,
                ReceiverAssets = Assets.From(argument.ReceiverAssets)
            };
        }
    }

    public class Transfer
    {
        public string Type { get; private set; }
        public string Name { get; private set; }
        public string Ticker { get; private set; }
        public string SvgUrl { get; private set; }
        public ESDTIdentifierValue Collection { get; private set; }
        public ESDTIdentifierValue Token { get; private set; }
        public int? Decimals { get; private set; }
        public ESDTIdentifierValue Identifier { get; private set; }
        public ESDTAmount Value { get; private set; }

        private Transfer() { }

        public static Transfer[] From(TransferDto[] transfers)
        {
            if (transfers == null) return null;

            return transfers.Select(transfer => new Transfer()
            {
                Type = transfer.Type,
                Name = transfer.Name,
                Ticker = transfer.Ticker,
                SvgUrl = transfer.SvgUrl,
                Collection = transfer.Collection is null ? null : ESDTIdentifierValue.From(transfer.Collection),
                Token = transfer.Token is null ? null : ESDTIdentifierValue.From(transfer.Token),
                Decimals = transfer.Decimals,
                Identifier = transfer.Identifier is null ? null : ESDTIdentifierValue.From(transfer.Identifier),
                Value = ESDTAmount.From(transfer.Value,
                                        ESDT.ESDT_TOKEN(transfer.Type, transfer.Name, transfer.Identifier ?? transfer.Token, transfer.Decimals ?? 0))
            }).ToArray();
        }
    }

    public class SmartContractResult
    {
        public string Hash { get; private set; }
        public DateTime ResultDate { get; private set; }
        public long Nonce { get; private set; }
        public GasLimit GasLimit { get; private set; }
        public long GasPrice { get; private set; }
        public ESDTAmount Value { get; private set; }
        public Address Sender { get; private set; }
        public Address Receiver { get; private set; }
        public Assets SenderAssets { get; private set; }
        public Assets ReceiverAssets { get; private set; }
        public string Data { get; private set; }
        public string PrevTxHash { get; private set; }
        public string OriginalTxHash { get; private set; }
        public string CallType { get; private set; }
        public string ReturnMessage { get; private set; }

        private SmartContractResult() { }

        public static SmartContractResult[] From(SmartContractResultDto[] scrs)
        {
            if (scrs == null) return null;

            return scrs.Select(scr => new SmartContractResult()
            {
                Hash = scr.Hash,
                ResultDate = scr.Timestamp.ToDateTime(),
                Nonce = scr.Nonce,
                GasLimit = new GasLimit(scr.GasLimit),
                GasPrice = scr.GasPrice,
                Value = ESDTAmount.From(scr.Value), //always EGLD
                Sender = Address.FromBech32(scr.Sender),
                Receiver = Address.FromBech32(scr.Receiver),
                SenderAssets = Assets.From(scr.SenderAssets),
                ReceiverAssets = Assets.From(scr.ReceiverAssets),
                Data = DataCoder.DecodeData(scr.Data),
                PrevTxHash = scr.PrevTxHash,
                OriginalTxHash = scr.OriginalTxHash,
                CallType = scr.CallType,
                ReturnMessage = scr.ReturnMessage
            }).ToArray();
        }
    }

    public class Log
    {
        public string Id { get; private set; }
        public Address Address { get; private set; }
        public Assets AddressAssets { get; private set; }
        public Event[] Events { get; private set; }

        private Log() { }

        public static Log From(LogDto log)
        {
            if (log == null) return null;

            return new Log()
            {
                Id = log.Id,
                Address = Address.FromBech32(log.Address),
                AddressAssets = Assets.From(log.AddressAssets),
                Events = Event.From(log.Events)
            };
        }
    }

    public class Event
    {
        public string Identifier { get; private set; }
        public Address Address { get; private set; }
        public string Data { get; private set; }
        public string[] Topics { get; private set; }
        public int? Order { get; private set; }
        public Assets AddressAssets { get; private set; }

        private Event() { }

        public static Event[] From(EventDto[] events)
        {
            if (events == null) return null;

            return events.Select(evnt => new Event()
            {
                Identifier = evnt.Identifier,
                Address = Address.FromBech32(evnt.Address),
                Data = DataCoder.DecodeData(evnt.Data),
                Topics = evnt.Topics,
                Order = evnt.Order,
                AddressAssets = Assets.From(evnt.AddressAssets)
            }).ToArray();
        }
    }

    public class Operation
    {
        public string Id { get; private set; }
        public string Action { get; private set; }
        public string Type { get; private set; }
        public string EsdtType { get; private set; }
        public ESDTIdentifierValue Collection { get; private set; }
        public ESDTIdentifierValue Identifier { get; private set; }
        public string Name { get; private set; }
        public Address Sender { get; private set; }
        public Address Receiver { get; private set; }
        public string Data { get; private set; }
        public ESDTAmount Value { get; private set; }
        public int? Decimals { get; private set; }
        public string SvgUrl { get; private set; }
        public Assets SenderAssets { get; private set; }
        public Assets ReceiverAssets { get; private set; }
        public string Message { get; private set; }

        private Operation() { }

        public static Operation[] From(OperationDto[] operations)
        {
            if (operations == null) return null;

            return operations.Select(operation => new Operation()
            {
                Id = operation.Id,
                Action = operation.Action,
                Type = operation.Type,
                EsdtType = operation.EsdtType,
                Collection = operation.Collection is null ? null : ESDTIdentifierValue.From(operation.Collection),
                Identifier = operation.Identifier is null ? null : ESDTIdentifierValue.From(operation.Identifier),
                Name = operation.Name,
                Sender = Address.FromBech32(operation.Sender),
                Receiver = Address.FromBech32(operation.Receiver),
                Data = operation.Data,
                Value = operation.Value is null ? null : ESDTAmount.ESDT(operation.Value,
                                                                         ESDT.ESDT_TOKEN(operation.EsdtType ?? operation.Type, operation.Name, operation.Identifier, operation.Decimals ?? 0)),
                Decimals = operation.Decimals,
                SvgUrl = operation.SvgUrl,
                SenderAssets = Assets.From(operation.SenderAssets),
                ReceiverAssets = Assets.From(operation.ReceiverAssets),
                Message = operation.Message
            }).ToArray();
        }
    }
}
