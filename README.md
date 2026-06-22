# Sworn Treasure

An AI-native, on-chain digging game built with Unity (WebGL) for **Zero Cup 2026** by 0G.

You play as a treasure hunter digging deep underground, managing energy, uncovering ancient artifacts, and racing for a spot on a fully on-chain leaderboard powered by **0G Chain**. An AI companion reacts to your run in real time, and every artifact you find gets its own AI-generated lore.

> Honest note: this started as a student game project and was extended with Web3 + AI for the hackathon. It is a real, working prototype, not a mockup.

---

## What makes it a 0G / AI-native app

| Pillar | How it is used today |
|---|---|
| **0G Chain** | Scores are written on-chain to a custom `SwornTreasureLeaderboard` smart contract on the 0G Galileo Testnet. The global leaderboard is read back directly from chain logs. No backend server. |
| **AI (LLM)** | An AI archaeologist generates short lore for each artifact you find, and an AI dog companion reacts to your run (going deep, low energy, game over, etc.). |
| **Wallet** | OKX wallet connect via an injected-provider bridge; the network auto-switches/adds 0G Galileo. |

### Live contract

- **Network:** 0G Galileo Testnet (chainId `16602` / `0x40DA`)
- **Contract:** `0x348B2366132436b981055f0BE12484F27cba3E66`
- **Explorer:** https://chainscan-galileo.0g.ai/address/0x348B2366132436b981055f0BE12484F27cba3E66

---

## Core gameplay loop

1. Connect your wallet on the main menu (required to play).
2. Dig down, left, or right through the tile-based underground. Every block costs energy; past depth 20 it costs double.
3. Collect gold (+5) and artifacts (+30 to +100). Rarer artifacts hide deeper.
4. Grab energy tiles, or surface and refill at the Shop for 50 points.
5. **Decide:** tap **Submit Score** to lock your points on-chain, or keep digging for more.
6. **The risk:** Submit Score is the *only* way to save. If you hit game over before submitting, your points are gone. Don't get greedy.

That last point turns the on-chain write from a chore into a genuine risk-reward decision, which is the heart of the game.

---

## Artifacts

Inspired by real Indonesian historical objects. Each has its own rarity, depth band, spawn chance, and reward.

| Rarity | Artifact | Main Depth | Reward |
|---|---|---:|---:|
| Common | Trowulan Pottery Shard | 0-40 | +30 |
| Uncommon | Gobog Wayang Coin | 40-80 | +40 |
| Rare | Bronze Funnel Axe | 80-120 | +60 |
| Ultra Rare | Bronze Ganesha Statue | 120-150 | +75 |
| Mythical | Wonoboyo Gold Bowl | 150-300 | +100 |

---

## Architecture

Unity WebGL talks to the browser (wallet + network) through a small JavaScript bridge (`.jslib`), since wallet extensions and `fetch` only exist in the browser layer.

```
Unity (C#)                       Browser (jslib)                 Network
──────────                       ───────────────                 ───────
ZeroGManager  ── ConnectWallet ─▶ window.okxwallet ───────────▶ 0G Galileo
              ── SubmitScore ───▶ eth_sendTransaction ────────▶ Leaderboard contract
              ◀─ OnLeaderboardData ── eth_getLogs ◀────────────  ScoreSubmitted events
AIService     ── CallAI ────────▶ fetch(OpenRouter) ──────────▶ LLM
```

Key scripts:

- `ZeroGManager.cs` - wallet state, score submit, leaderboard fetch/parse (persists across scenes).
- `ZeroGBridge.jslib` - browser bridge for wallet, transactions, chain logs, and AI calls.
- `SwornTreasureLeaderboard.sol` - the on-chain leaderboard contract.
- `AIService.cs` / `DogCompanion.cs` / `ArtifactLoreUI.cs` - AI lore + companion.
- `InGameSubmitButton.cs` / `LeaderboardUI.cs` / `HowToPlayPanel.cs` - runtime-built UI.
- `MiningSystem.cs` - the core digging, energy, shop, and artifact logic.

---

## Tech stack

- Unity 6 (WebGL), C#
- Solidity (0G Galileo EVM)
- ethers.js + solc (contract compile & deploy)
- OKX Wallet (injected EVM provider)
- OpenRouter (LLM inference, current round)
- Unity Tilemap, TextMesh Pro

---

## Running it

### In the Unity Editor

1. Open the project in Unity 6.
2. Open `Assets/Scenes/main-menu.unity` and press Play. (Wallet/AI run only in real WebGL builds; the Editor uses safe fallbacks.)

### WebGL build

```
Platform:           WebGL
Managed Stripping:  Minimal   (SendMessage callbacks must not be stripped)
Compression:        Disabled
Canvas:             1280 x 720
Color Space:        Gamma
```

### Test locally

```bash
cd <build-output-folder>
python -m http.server 8080
# open http://localhost:8080  (not file://, wallet needs a real origin)
```

---

## Roadmap

Current round covers **0G Chain + AI**. If we advance, the plan is to go deeper into the 0G stack and make the game properly AI-native end-to-end.

### Next round - go fully on 0G

- **Move AI to 0G Compute.** Replace OpenRouter with 0G's decentralized inference so artifact lore and the dog companion run on 0G infrastructure, not a third-party API. This closes the biggest gap and makes the "AI-native on 0G" claim literal.
- **0G Storage for content.** Persist AI-generated lore, artifact metadata, and run history on 0G Storage so discoveries are permanent and shareable, not regenerated each time.
- **On-chain artifacts as NFTs.** Mint rare finds (Ultra Rare / Mythical) as ERC-721 tokens on 0G Chain, with the AI lore stored on 0G Storage as the token metadata.
- **Dog skins / breeds as NFTs.** Turn the existing skin system into ownable ERC-721 dog breeds on 0G Chain. Players own, equip, and trade their companion, with each breed's art and traits stored on 0G Storage.

### Content - a world of treasures

- **Global artifact set.** Right now every artifact is Indonesian. Next round we broaden the pool into a worldwide collection (Egyptian, Mesoamerican, Mediterranean, East Asian, and more) so discoveries feel varied and universal instead of tied to a single culture. Pairs naturally with AI-generated lore, which can describe any artifact on the fly.

### Later - depth and fairness

- **Provably fair loot** via on-chain verifiable randomness (VRF) so drop rates can't be gamed.
- **Seasonal leaderboards** with on-chain seasons and reward distribution to top players.
- **Anti-cheat** score submission (commit-reveal or signed runs) so the leaderboard stays trustworthy.
- **AI-generated artifact art** using 0G Compute image inference, so every artifact can look unique.
- **Player economy** - a simple marketplace to trade minted artifacts.

---

## Status

Working prototype, submitted to Zero Cup 2026. Playable end to end: connect wallet, dig, submit on-chain, and view the global leaderboard.

## Author

Darko
