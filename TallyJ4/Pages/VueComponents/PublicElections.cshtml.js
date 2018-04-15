Vue.component('public-elections', {
  template: '#public-elections',
  data: function () {
    return {
      listing: [
        {
          guid: '123',
          name: 'Election 1'
        },
        {
          guid: '1234',
          name: 'Election 2'
        }],
      election: '',
      joinStatus: '',
      passcode: '',
      joinStatusClass: 'idle'
    }
  },
  computed: {
    selectSize: function () {
      return Math.max(4, Math.min(10, this.listing.length + 1));
    }
  },
  created: function () {
    this.connectToPublicHub();
  },
  mounted: function () {
    console.log('mount');
    this.listing = window._publicElections || [];
  },
  methods: {
    connectToPublicHub: function () {
      var vue = this;

      var hub = new signalR.HubConnection('/public');

      hub.on('electionsListUpdated', function (listing) {
        console.log('signalR: electionsListUpdated', listing);

        vue.updateListing(listing);
      });

      hub.onclose(function () {
        console.log('closed... restarting...');
        hub.start().catch(msg => function (msg) {
          console.log('error next connection')
          console.log(msg);
        });
      });

      hub.start().catch(msg => function (msg) {
        console.log('error first connection')
        console.log(msg);
      });
    },
    updateListing: function (listing) {
      var vue = this;

      vue.listing = listing || [];
      vue.selectDefaultElection();
    },
    selectDefaultElection: function () {
      var vue = this;

      if (vue.listing.length == 1) {
        vue.election = vue.listing[0].value;
      }
    },
    joinNow: function () {
      var vue = this;
      console.log('join', this.election);

      vue.joinStatusClass = 'active';
      vue.joinStatus = 'Checking...';

      var form = {
        electionGuid: vue.election,
        passcode: vue.passcode,
        oldCompGuid: GetFromStorage('compcode_' + vue.election, null)
      };

      CallAjaxHandler(_urlTellerJoin, form, function (info) {
        if (info.loggedIn) {
          SetInStorage('compcode_' + electionGuid, info.compGuid);

          vue.joinStatusClass = 'success';
          vue.joinStatus = 'Success! &nbsp; Going to the Dashboard now...';

          location.href = _urlGuestTellerHome;
          return;
        }

        vue.refreshList();

        vue.joinStatusClass = 'error';
        vue.joinStatus = info.error;
      });
    },
    refreshList: function () {
      var vue = this;
      vue.joinStatusClass = 'idle';
      vue.joinStatus = '';
      vue.listing.splice(1);
      vue.listing[0].text = 'Loading...';

      CallAjaxHandler(_urlOpenElections, null, function (info) {
        vue.updateListing(info);
      });
    }

  }
});

