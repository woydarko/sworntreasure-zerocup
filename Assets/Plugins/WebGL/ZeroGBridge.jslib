mergeInto(LibraryManager.library, {

  ConnectWallet: function () {
    var provider = window.okxwallet || window.ethereum;
    if (!provider) {
      SendMessage('ZeroGManager', 'OnWalletError', 'No wallet found. Install OKX Wallet.');
      return;
    }

    var zeroGChain = {
      chainId: '0x40DA',
      chainName: '0G Galileo Testnet',
      nativeCurrency: { name: '0G', symbol: 'OG', decimals: 18 },
      rpcUrls: ['https://evmrpc-testnet.0g.ai'],
      blockExplorerUrls: ['https://chainscan-galileo.0g.ai']
    };

    provider.request({ method: 'eth_requestAccounts' })
      .then(function (accounts) {
        var address = accounts[0];
        return provider.request({
          method: 'wallet_switchEthereumChain',
          params: [{ chainId: zeroGChain.chainId }]
        })
        .catch(function (switchErr) {
          if (switchErr.code === 4902 || switchErr.code === -32603) {
            return provider.request({
              method: 'wallet_addEthereumChain',
              params: [zeroGChain]
            });
          }
          throw switchErr;
        })
        .then(function () {
          SendMessage('ZeroGManager', 'OnWalletConnected', address);
        });
      })
      .catch(function (err) {
        SendMessage('ZeroGManager', 'OnWalletError', err.message || 'Wallet connect failed');
      });
  },

  SubmitScore: function (addressPtr, scorePtr) {
    var address = UTF8ToString(addressPtr);
    var score = parseInt(UTF8ToString(scorePtr));
    var provider = window.okxwallet || window.ethereum;
    var CONTRACT = '0x348B2366132436b981055f0BE12484F27cba3E66';

    if (!provider) {
      SendMessage('ZeroGManager', 'OnSubmitError', 'No wallet found');
      return;
    }

    // ABI-encode submitScore(uint256): selector + 32-byte padded score
    var scorePadded = score.toString(16).padStart(64, '0');
    var data = '0xaff0b297' + scorePadded;

    provider.request({
      method: 'eth_sendTransaction',
      params: [{
        from: address,
        to: CONTRACT,
        value: '0x0',
        data: data
      }]
    })
    .then(function (txHash) {
      SendMessage('ZeroGManager', 'OnScoreSubmitted', txHash);
    })
    .catch(function (err) {
      SendMessage('ZeroGManager', 'OnSubmitError', err.message || 'Submit failed');
    });
  },

  FetchLeaderboard: function (_unused) {
    var CONTRACT = '0x348B2366132436b981055f0BE12484F27cba3E66';
    var RPC = 'https://evmrpc-testnet.0g.ai';
    var EVENT_TOPIC = '0xb7f20d0949b6a8bc59d005af4a52f7ff5d0cfcde9056fa556adb0e4b24dcb6d2';

    fetch(RPC, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        jsonrpc: '2.0',
        method: 'eth_getLogs',
        params: [{
          address: CONTRACT,
          topics: [EVENT_TOPIC],
          fromBlock: '0x0',
          toBlock: 'latest'
        }],
        id: 1
      })
    })
    .then(function (res) { return res.json(); })
    .then(function (data) {
      var results = [];
      if (!data.result) { SendMessage('ZeroGManager', 'OnLeaderboardData', '[]'); return; }

      data.result.forEach(function (log) {
        try {
          // topics[1] = indexed address (padded 32 bytes), data = score + timestamp (32 bytes each)
          var addrHex = '0x' + log.topics[1].slice(26);
          var scoreHex = log.data.slice(2, 66);
          var tsHex = log.data.slice(66, 130);
          var scoreVal = parseInt(scoreHex, 16);
          var timestamp = parseInt(tsHex, 16);
          results.push({ address: addrHex, score: scoreVal, timestamp: timestamp });
        } catch (e) { /* skip malformed */ }
      });

      results.sort(function (a, b) { return b.score - a.score; });
      SendMessage('ZeroGManager', 'OnLeaderboardData', JSON.stringify(results.slice(0, 10)));
    })
    .catch(function () {
      SendMessage('ZeroGManager', 'OnLeaderboardData', '[]');
    });
  },

  InitAI: function (apiKeyPtr, modelPtr) {
    window._orApiKey = UTF8ToString(apiKeyPtr);
    window._orModel = UTF8ToString(modelPtr);
  },

  CallAI: function (callbackNamePtr, promptPtr) {
    var callbackName = UTF8ToString(callbackNamePtr);
    var prompt = UTF8ToString(promptPtr);

    if (!window._orApiKey) {
      SendMessage('AIService', 'OnAIError', 'API key not set');
      return;
    }

    fetch('https://openrouter.ai/api/v1/chat/completions', {
      method: 'POST',
      headers: {
        'Authorization': 'Bearer ' + window._orApiKey,
        'Content-Type': 'application/json',
        'HTTP-Referer': window.location.href,
        'X-Title': 'Sworn Treasure'
      },
      body: JSON.stringify({
        model: window._orModel || 'meta-llama/llama-3.1-8b-instruct',
        messages: [{ role: 'user', content: prompt }],
        max_tokens: 80
      })
    })
    .then(function (res) { return res.json(); })
    .then(function (data) {
      var text = data.choices && data.choices[0] && data.choices[0].message
        ? data.choices[0].message.content.trim()
        : 'Something ancient stirs...';
      SendMessage('AIService', callbackName, text);
    })
    .catch(function (err) {
      SendMessage('AIService', 'OnAIError', err.message || 'AI call failed');
    });
  }

});
