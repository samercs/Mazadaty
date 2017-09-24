namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateBidStoredProcs : DbMigration
    {
        public override void Up()
        {
            Sql(@"
create procedure [dbo].[SubmitAutoBid]
(
	@AuctionId		int,
	@SecondsLeft	int,
	@UserId			nvarchar(128) output,
	@BidId			int output,
	@Amount			float output,
	@UserName		nvarchar(max) output,
	@AvatarUrl		nvarchar(max) output
)
as

begin

set nocount on

declare @BidIncrement float
declare @AutoBidUserId varchar(128)
declare @LastBidUserId varchar(128)
declare @LastBidAmount float

select	@AuctionId = AuctionId,
		@BidIncrement = BidIncrement
from	Auctions
where	AuctionId = @AuctionId and
		Status = 2

if (@AuctionId is null) begin

	print ('Cannot process autobid for specified auction ID.');
	return;

end

begin transaction

-- get last bid user and amount, lock table during transaction
select top 1
		@LastBidUserId = UserId,
		@LastBidAmount = Amount
from	Bids
with	(TABLOCKX)
where	AuctionId = @AuctionId
order by BidId desc

set @LastBidAmount = isnull(@LastBidAmount, 0);

-- get new autobid user
select top 1
		@AutoBidUserId  = UserId
from	AutoBids
where	AuctionId = @AuctionId and
		(@LastBidUserId is null or UserId <> @LastBidUserId) and
		MaxBid > @LastBidAmount
order by NEWID()

-- if no autobids are configured return
if (@AutoBidUserId is null) begin

	print ('No autobid user available to bid on this auction.');
	commit transaction;
	return;

end

declare @BidAmount float = @LastBidAmount + @BidIncrement;

insert into Bids (UserId, AuctionId, Amount, SecondsLeft, UserHostAddress, Type) values  (@AutoBidUserId, @AuctionId, @BidAmount, @SecondsLeft, '', 0);

select 	@BidId = b.BidId,
		@Amount = b.Amount,
		@UserId = b.UserId,
		@UserName = u.UserName,
		@AvatarUrl = u.AvatarUrl
from	Bids b
		inner join AspNetUsers u on b.UserId = u.Id
where	b.BidId = @@IDENTITY

commit transaction;

end");

            Sql(@"
create procedure [dbo].[SubmitUserBid]
(
	@AuctionId		int,
	@SecondsLeft	int,
	@UserId			nvarchar(128) output,
	@BidId			int output,
	@Amount			float output,
	@UserName		nvarchar(max) output,
	@AvatarUrl		nvarchar(max) output
)
as

begin

set nocount on

declare @BidIncrement float
declare @LastBidUserId varchar(128)
declare @LastBidAmount float

select	@AuctionId = AuctionId,
		@BidIncrement = BidIncrement
from	Auctions
where	AuctionId = @AuctionId and
		Status = 2

if (@AuctionId is null) begin

	print ('Cannot process autobid for specified auction ID.');
	return;

end

begin transaction

-- get last bid user and amount, lock table during transaction
select top 1
		@LastBidUserId = UserId,
		@LastBidAmount = Amount
from	Bids
with	(TABLOCKX)
where	AuctionId = @AuctionId
order by BidId desc

-- check duplicate user bid
if (@LastBidUserId = @UserId) begin

	print ('User cannot bid twice in a row.');
	commit transaction;
	return;

end

declare @BidAmount float = isnull(@LastBidAmount, 0) + @BidIncrement;

insert into Bids (UserId, AuctionId, Amount, SecondsLeft, UserHostAddress, Type) values  (@UserId, @AuctionId, @BidAmount, @SecondsLeft, '', 1);

select 	@BidId = b.BidId,
		@Amount = b.Amount,
		@UserId = b.UserId,
		@UserName = u.UserName,
		@AvatarUrl = u.AvatarUrl
from	Bids b
		inner join AspNetUsers u on b.UserId = u.Id
where	b.BidId = @@IDENTITY

commit transaction;

end");

        }
        
        public override void Down()
        {
        }
    }
}
