var Auction = function (data) {
    var self = this;

    self.auctionId = data.auctionId;
    self.title = data.title;
    self.description = data.description;
    self.specifications = data.specifications;
    self.retailPrice = data.retailPrice;
    self.buyNowEnabled = data.buyNowEnabled;
    self.images = data.images;

    self.status = ko.observable(data.status);
    self.isLoaded = ko.observable(false);
    self.timeLeft = ko.observable();
    self.lastBidAmount = ko.observable(data.lastBidAmount);
    self.lastBidUser = ko.observable(data.lastBidUser);
    self.startUtc = ko.observable(data.startUtc);

    self.displayTemplate = function () {
        switch (self.status()) {
            case "Live":
                return "live-template";
            case "Upcoming":
                return "upcoming-template";
            case "Closed":
                return "closed-template";
            default:
                throw "Invalid auction status.";
        }
    }

    self.buyNowUrl = ko.computed(function () {
        return `orders/buy-now/${self.auctionId}`;
    });

    self.timeLeftFormatted = ko.computed(function () {
        return TimeUtilities.getTimeLeft(self.timeLeft());
    });

    self.lastBidFormatted = ko.computed(function () {
        if (!self.lastBidUser()) {
            return "";
        }

        if (self.status() === "Closed") {
            return "Winning Bid: KD " + self.lastBidAmount() + " by " + self.lastBidUser();
        }

        return "Current Bid: KD " + self.lastBidAmount() + " by " + self.lastBidUser();
    });

    self.startTimeFormatted = ko.computed(() => ("Starting: " + moment(this.startUtc()).format('D MMM HH:mm')));

    self.submitBid = () => {
        //auctionHub.server.submitBid(self.auctionId);
    };

    self.closeAuction = () => {
        this.status("Closed");
    };

    self.redirectOrder = orderId => {
        location.href = `orders/auction/${orderId}`;
    };
};

var AuctionsViewModel = function (data) {
    var self = this;
    self.auctions = ko.observableArray([]);

    self.getAuction = auctionId => {
        for (var i = 0, len = this.auctions().length; i < len; i++) {
            if (this.auctions()[i].auctionId === auctionId)
                return this.auctions()[i];
        }
        return null;
    };

    var mappedAuctions = $.map(data, auction => new Auction(auction));
    self.auctions(mappedAuctions);
};

