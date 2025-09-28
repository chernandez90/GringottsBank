<template>
  <div class="atm-container">
    <div class="atm-frame">
      <div class="atm-header">
        <div class="logo-container">
          <img
            src="/gringotts-logo.png"
            alt="Gringotts Logo"
            class="gringotts-logo"
            @error="handleLogoError"
          />
          <div v-if="showFallbackLogo" class="goblin-seal-fallback">🏛️</div>
        </div>
        <h1 class="bank-title">
          <span class="gold-text">GRINGOTTS</span>
          <span class="subtitle">WIZARDING BANK</span>
        </h1>
      </div>

      <div class="atm-screen">
        <!-- Navigation Bar (shows on all screens except menu) -->
        <div v-if="currentScreen !== 'menu'" class="navigation-bar">
          <button @click="goToMenu" class="nav-btn">← Main Menu</button>
          <span class="current-screen-title">{{ getScreenTitle() }}</span>
        </div>

        <!-- Main Menu -->
        <div v-if="currentScreen === 'menu'" class="screen menu-screen">
          <h2 class="welcome-msg">Welcome to Your Vault</h2>
          <div class="account-info">
            <div class="account-card" v-if="checkingAccount">
              <h3>🏦 Checking Vault</h3>
              <p class="balance">
                {{ formatCurrency(checkingAccount.balance) }}
              </p>
            </div>
            <div class="account-card" v-if="savingsAccount">
              <h3>💎 Savings Vault</h3>
              <p class="balance">
                {{ formatCurrency(savingsAccount.balance) }}
              </p>
            </div>
          </div>
          <div class="menu-options">
            <button @click="showWithdraw" class="menu-btn">
              💰 Withdraw Gold
            </button>
            <button @click="showDeposit" class="menu-btn">
              💎 Deposit Treasure
            </button>
            <button @click="showTransfer" class="menu-btn">
              🔄 Transfer Between Vaults
            </button>
            <button @click="showHistory" class="menu-btn">
              📜 Transaction History
            </button>
          </div>
        </div>

        <!-- Withdraw Screen -->
        <div
          v-if="currentScreen === 'withdraw'"
          class="screen transaction-screen"
        >
          <h3>💰 Withdraw Gold</h3>
          <div class="account-selector">
            <label>Select Vault:</label>
            <div class="account-options">
              <button
                :class="[
                  'account-btn',
                  { active: selectedAccount?.id === checkingAccount?.id },
                ]"
                @click="selectAccount(checkingAccount)"
                v-if="checkingAccount"
              >
                🏦 Checking<br />{{ formatCurrency(checkingAccount.balance) }}
              </button>
              <button
                :class="[
                  'account-btn',
                  { active: selectedAccount?.id === savingsAccount?.id },
                ]"
                @click="selectAccount(savingsAccount)"
                v-if="savingsAccount"
              >
                💎 Savings<br />{{ formatCurrency(savingsAccount.balance) }}
              </button>
            </div>
          </div>
          <div v-if="selectedAccount" class="amount-section">
            <p class="available-balance">
              Available: {{ formatCurrency(selectedAccount.balance) }}
            </p>
            <div class="quick-amounts">
              <button
                v-for="amount in quickAmounts"
                :key="amount"
                @click="setAmount(amount)"
                :disabled="amount > selectedAccount.balance"
                class="amount-btn"
              >
                {{ formatCurrency(amount) }}
              </button>
            </div>
            <div class="custom-amount">
              <label>Custom Amount:</label>
              <input
                v-model.number="customAmount"
                type="number"
                step="0.01"
                placeholder="Enter amount"
                class="magical-input"
              />
            </div>
            <div class="button-group">
              <button
                @click="processWithdraw"
                :disabled="!canWithdraw"
                class="magical-btn primary"
              >
                ✨ Summon Gold
              </button>
              <button @click="goToMenu" class="magical-btn secondary">
                ← Back to Menu
              </button>
            </div>
          </div>
        </div>

        <!-- Deposit Screen -->
        <div
          v-if="currentScreen === 'deposit'"
          class="screen transaction-screen"
        >
          <h3>💎 Deposit Treasure</h3>
          <div class="account-selector">
            <label>Select Vault:</label>
            <div class="account-options">
              <button
                :class="[
                  'account-btn',
                  { active: selectedAccount?.id === checkingAccount?.id },
                ]"
                @click="selectAccount(checkingAccount)"
                v-if="checkingAccount"
              >
                🏦 Checking<br />{{ formatCurrency(checkingAccount.balance) }}
              </button>
              <button
                :class="[
                  'account-btn',
                  { active: selectedAccount?.id === savingsAccount?.id },
                ]"
                @click="selectAccount(savingsAccount)"
                v-if="savingsAccount"
              >
                💎 Savings<br />{{ formatCurrency(savingsAccount.balance) }}
              </button>
            </div>
          </div>
          <div v-if="selectedAccount" class="amount-section">
            <div class="custom-amount">
              <label>Amount to Deposit:</label>
              <input
                v-model.number="customAmount"
                type="number"
                step="0.01"
                placeholder="Enter amount"
                class="magical-input"
              />
            </div>
            <div class="button-group">
              <button
                @click="processDeposit"
                :disabled="!customAmount || customAmount <= 0"
                class="magical-btn primary"
              >
                🏛️ Store in Vault
              </button>
              <button @click="goToMenu" class="magical-btn secondary">
                ← Back to Menu
              </button>
            </div>
          </div>
        </div>

        <!-- Transfer Screen -->
        <div
          v-if="currentScreen === 'transfer'"
          class="screen transaction-screen"
        >
          <h3>🔄 Transfer Between Vaults</h3>
          <div class="transfer-section">
            <div class="transfer-accounts">
              <div class="from-account">
                <label>From:</label>
                <div class="account-options vertical">
                  <button
                    :class="[
                      'account-btn',
                      { active: fromAccount?.id === checkingAccount?.id },
                    ]"
                    @click="fromAccount = checkingAccount"
                    v-if="checkingAccount"
                  >
                    🏦 Checking<br />{{
                      formatCurrency(checkingAccount.balance)
                    }}
                  </button>
                  <button
                    :class="[
                      'account-btn',
                      { active: fromAccount?.id === savingsAccount?.id },
                    ]"
                    @click="fromAccount = savingsAccount"
                    v-if="savingsAccount"
                  >
                    💎 Savings<br />{{ formatCurrency(savingsAccount.balance) }}
                  </button>
                </div>
              </div>
              <div class="transfer-arrow">→</div>
              <div class="to-account">
                <label>To:</label>
                <div class="account-options vertical">
                  <button
                    :class="[
                      'account-btn',
                      { active: toAccount?.id === checkingAccount?.id },
                    ]"
                    @click="toAccount = checkingAccount"
                    v-if="
                      checkingAccount && fromAccount?.id !== checkingAccount?.id
                    "
                  >
                    🏦 Checking<br />{{
                      formatCurrency(checkingAccount.balance)
                    }}
                  </button>
                  <button
                    :class="[
                      'account-btn',
                      { active: toAccount?.id === savingsAccount?.id },
                    ]"
                    @click="toAccount = savingsAccount"
                    v-if="
                      savingsAccount && fromAccount?.id !== savingsAccount?.id
                    "
                  >
                    💎 Savings<br />{{ formatCurrency(savingsAccount.balance) }}
                  </button>
                </div>
              </div>
            </div>
            <div v-if="fromAccount && toAccount" class="amount-section">
              <p class="available-balance">
                Available: {{ formatCurrency(fromAccount.balance) }}
              </p>
              <div class="custom-amount">
                <label>Amount to Transfer:</label>
                <input
                  v-model.number="customAmount"
                  type="number"
                  step="0.01"
                  :max="fromAccount.balance"
                  placeholder="Enter amount"
                  class="magical-input"
                />
              </div>
              <div class="button-group">
                <button
                  @click="processTransfer"
                  :disabled="!canTransfer"
                  class="magical-btn primary"
                >
                  🦉 Send by Owl
                </button>
                <button @click="goToMenu" class="magical-btn secondary">
                  ← Back to Menu
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Transaction History Screen -->
        <div v-if="currentScreen === 'history'" class="screen history-screen">
          <h3>📜 Transaction History</h3>
          <div class="account-selector">
            <label>Select Vault:</label>
            <div class="account-options">
              <button
                :class="[
                  'account-btn',
                  { active: selectedAccount?.id === checkingAccount?.id },
                ]"
                @click="selectAccountForHistory(checkingAccount)"
                v-if="checkingAccount"
              >
                🏦 Checking
              </button>
              <button
                :class="[
                  'account-btn',
                  { active: selectedAccount?.id === savingsAccount?.id },
                ]"
                @click="selectAccountForHistory(savingsAccount)"
                v-if="savingsAccount"
              >
                💎 Savings
              </button>
              <button
                :class="['account-btn', { active: selectedAccount === 'all' }]"
                @click="
                  selectedAccount = 'all';
                  loadAllTransactions();
                "
              >
                📋 All Vaults
              </button>
            </div>
          </div>
          <div class="transaction-list" v-if="transactions.length > 0">
            <div
              v-for="transaction in transactions"
              :key="transaction.id"
              class="transaction-item"
            >
              <div class="transaction-header">
                <span class="transaction-type"
                  >{{ getTransactionIcon(transaction.transactionType) }}
                  {{ transaction.transactionType }}</span
                >
                <span class="transaction-date">{{
                  formatDate(transaction.transactionDate)
                }}</span>
              </div>
              <div class="transaction-details">
                <span
                  :class="[
                    'transaction-amount',
                    getAmountClass(transaction.transactionType),
                  ]"
                >
                  {{ formatCurrency(transaction.amount) }}
                </span>
                <span class="transaction-account">
                  {{ transaction.accountType }}
                </span>
              </div>
            </div>
          </div>
          <div v-else-if="selectedAccount" class="no-transactions">
            <p>No transactions found for this vault.</p>
          </div>
          <div class="button-group">
            <button @click="goToMenu" class="magical-btn secondary">
              ← Back to Menu
            </button>
          </div>
        </div>

        <!-- Result Screen -->
        <div v-if="currentScreen === 'result'" class="screen result-screen">
          <div class="result-content">
            <div v-if="transactionSuccess" class="success">
              <h3>✅ Transaction Successful</h3>
              <p>{{ transactionMessage }}</p>
              <div v-if="updatedBalances" class="updated-balances">
                <div v-if="updatedBalances.checking" class="balance-update">
                  🏦 Checking: {{ formatCurrency(updatedBalances.checking) }}
                </div>
                <div v-if="updatedBalances.savings" class="balance-update">
                  💎 Savings: {{ formatCurrency(updatedBalances.savings) }}
                </div>
              </div>
            </div>
            <div v-else class="error">
              <h3>❌ Transaction Failed</h3>
              <p>{{ transactionMessage }}</p>
            </div>
            <button @click="goToMenu" class="magical-btn">
              Return to Menu
            </button>
          </div>
        </div>

        <!-- Loading Screen -->
        <div v-if="loading" class="screen loading-screen">
          <div class="spinner">⚡</div>
          <p>The goblins are processing your request...</p>
        </div>
      </div>

      <div class="atm-footer">
        <p class="security-notice">
          🛡️ Protected by Goblin Magic & Dragon Fire
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import { BankApiService } from "../services/bankApi.js";

// Reactive data
const currentScreen = ref("menu");
const loading = ref(false);
const checkingAccount = ref(null);
const savingsAccount = ref(null);
const selectedAccount = ref(null);
const fromAccount = ref(null);
const toAccount = ref(null);
const customAmount = ref(null);
const transactions = ref([]);
const transactionSuccess = ref(false);
const transactionMessage = ref("");
const updatedBalances = ref(null);
const showFallbackLogo = ref(false);

// Quick withdrawal amounts
const quickAmounts = [20, 50, 100, 200];

// Computed properties
const canWithdraw = computed(() => {
  return (
    selectedAccount.value &&
    customAmount.value > 0 &&
    customAmount.value <= selectedAccount.value.balance
  );
});

const canTransfer = computed(() => {
  return (
    fromAccount.value &&
    toAccount.value &&
    customAmount.value > 0 &&
    customAmount.value <= fromAccount.value.balance
  );
});

// Methods
const loadAccounts = async () => {
  try {
    loading.value = true;
    const accounts = await BankApiService.getAllAccounts();

    checkingAccount.value = accounts.find(
      (acc) => acc.type.toLowerCase() === "checking"
    );
    savingsAccount.value = accounts.find(
      (acc) => acc.type.toLowerCase() === "savings"
    );
  } catch (error) {
    console.error("Failed to load accounts:", error);
  } finally {
    loading.value = false;
  }
};

const showWithdraw = () => {
  currentScreen.value = "withdraw";
  selectedAccount.value = null;
  customAmount.value = null;
};

const showDeposit = () => {
  currentScreen.value = "deposit";
  selectedAccount.value = null;
  customAmount.value = null;
};

const showTransfer = () => {
  currentScreen.value = "transfer";
  fromAccount.value = null;
  toAccount.value = null;
  customAmount.value = null;
};

const showHistory = () => {
  currentScreen.value = "history";
  selectedAccount.value = null;
  transactions.value = [];
};

const selectAccount = (account) => {
  selectedAccount.value = account;
};

const selectAccountForHistory = (account) => {
  selectedAccount.value = account;
  loadTransactions(account.id);
};

const setAmount = (amount) => {
  customAmount.value = amount;
};

const processWithdraw = async () => {
  if (!canWithdraw.value) return;

  try {
    loading.value = true;
    const transactionData = {
      accountId: selectedAccount.value.id,
      amount: customAmount.value,
      transactionType: "Withdrawal",
    };

    await BankApiService.createTransaction(transactionData);

    // Refresh account data from API
    await loadAccounts();

    transactionSuccess.value = true;
    transactionMessage.value = `Successfully withdrew ${formatCurrency(
      customAmount.value
    )} from your ${selectedAccount.value.type} vault.`;

    // Update the updatedBalances for display
    updatedBalances.value = {
      checking: checkingAccount.value?.balance,
      savings: savingsAccount.value?.balance,
    };

    currentScreen.value = "result";
  } catch (error) {
    console.error("Withdrawal error:", error);
    transactionSuccess.value = false;
    transactionMessage.value =
      error.message ||
      "Connection failed. The goblins are having technical difficulties.";
    currentScreen.value = "result";
  } finally {
    loading.value = false;
  }
};

const processDeposit = async () => {
  if (!customAmount.value || customAmount.value <= 0) return;

  try {
    loading.value = true;
    const transactionData = {
      accountId: selectedAccount.value.id,
      amount: customAmount.value,
      transactionType: "Deposit",
    };

    await BankApiService.createTransaction(transactionData);

    // Refresh account data from API
    await loadAccounts();

    transactionSuccess.value = true;
    transactionMessage.value = `Successfully deposited ${formatCurrency(
      customAmount.value
    )} to your ${selectedAccount.value.type} vault.`;

    // Update the updatedBalances for display
    updatedBalances.value = {
      checking: checkingAccount.value?.balance,
      savings: savingsAccount.value?.balance,
    };

    currentScreen.value = "result";
  } catch (error) {
    console.error("Deposit error:", error);
    transactionSuccess.value = false;
    transactionMessage.value =
      error.message ||
      "Connection failed. The goblins are having technical difficulties.";
    currentScreen.value = "result";
  } finally {
    loading.value = false;
  }
};

const processTransfer = async () => {
  if (!canTransfer.value) return;

  try {
    loading.value = true;
    const transferData = {
      fromAccountId: fromAccount.value.id,
      toAccountId: toAccount.value.id,
      amount: customAmount.value,
    };

    await BankApiService.transferFunds(transferData);

    // Refresh account data from API
    await loadAccounts();

    transactionSuccess.value = true;
    transactionMessage.value = `Successfully transferred ${formatCurrency(
      customAmount.value
    )} from ${fromAccount.value.type} to ${toAccount.value.type} vault.`;

    // Update the updatedBalances for display
    updatedBalances.value = {
      checking: checkingAccount.value?.balance,
      savings: savingsAccount.value?.balance,
    };

    currentScreen.value = "result";
  } catch (error) {
    console.error("Transfer error:", error);
    transactionSuccess.value = false;
    transactionMessage.value =
      error.message ||
      "Connection failed. The goblins are having technical difficulties.";
    currentScreen.value = "result";
  } finally {
    loading.value = false;
  }
};

const loadTransactions = async (accountId) => {
  try {
    loading.value = true;
    const data = await BankApiService.getTransactionsByAccount(accountId);
    transactions.value = data.sort(
      (a, b) => new Date(b.timestamp) - new Date(a.timestamp)
    );
  } catch (error) {
    console.error("Failed to load transactions:", error);
    transactions.value = [];
  } finally {
    loading.value = false;
  }
};

const loadAllTransactions = async () => {
  try {
    loading.value = true;
    const data = await BankApiService.getAllTransactions();
    transactions.value = data.sort(
      (a, b) => new Date(b.timestamp) - new Date(a.timestamp)
    );
  } catch (error) {
    console.error("Failed to load transactions:", error);
    transactions.value = [];
  } finally {
    loading.value = false;
  }
};

const goToMenu = () => {
  currentScreen.value = "menu";
  selectedAccount.value = null;
  fromAccount.value = null;
  toAccount.value = null;
  customAmount.value = null;
  transactions.value = [];
  updatedBalances.value = null;
};

const formatCurrency = (amount) => {
  return `${amount.toFixed(2)} G`; // G for Galleons
};

const formatDate = (dateString) => {
  if (!dateString) return "N/A";

  try {
    // Parse the UTC date string and convert to local time
    const utcDate = new Date(dateString);

    // Check if the date is valid
    if (isNaN(utcDate.getTime())) {
      return "Invalid Date";
    }

    // Format in local timezone with readable format
    return utcDate.toLocaleString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
      timeZoneName: "short",
    });
  } catch (error) {
    console.error("Error formatting date:", error);
    return "Invalid Date";
  }
};

const getTransactionIcon = (type) => {
  const icons = {
    Deposit: "💰",
    Withdrawal: "🏛️",
    Transfer: "🔄",
    "Transfer In": "📥",
    "Transfer Out": "📤",
  };
  return icons[type] || "📋";
};

const getAmountClass = (type) => {
  return type === "Deposit" || type === "Transfer In" ? "positive" : "negative";
};

const getScreenTitle = () => {
  const titles = {
    withdraw: "💰 Withdraw Gold",
    deposit: "💎 Deposit Treasure",
    transfer: "🔄 Transfer Between Vaults",
    history: "📜 Transaction History",
    result: "Transaction Result",
    loading: "Processing...",
  };
  return titles[currentScreen.value] || "";
};

const handleLogoError = () => {
  showFallbackLogo.value = true;
};

onMounted(() => {
  loadAccounts();
});
</script>

<style scoped>
@import url("https://fonts.googleapis.com/css2?family=Cinzel:wght@400;600;700&family=Uncial+Antiqua&display=swap");

.atm-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: radial-gradient(ellipse at center, #3e2723 0%, #1a0e0a 100%),
    url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" width="100" height="100" viewBox="0 0 100 100"><defs><filter id="noise"><feTurbulence baseFrequency="0.9" numOctaves="4" stitchTiles="stitch"/><feColorMatrix values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0"/></filter></defs><rect width="100%" height="100%" filter="url(%23noise)" opacity="0.3"/></svg>');
  padding: 20px;
  font-family: "Cinzel", serif;
}

.atm-frame {
  width: 700px;
  max-width: 95vw;
  background: 
    /* Main parchment color */ radial-gradient(
      ellipse at 20% 30%,
      #f5e6d3 0%,
      #e8d5b7 25%,
      #dfc49a 50%,
      #d4b896 75%,
      #c9a876 100%
    ),
    /* Age spots and stains */
      radial-gradient(
        circle at 10% 20%,
        rgba(139, 69, 19, 0.1) 0%,
        transparent 10%
      ),
    radial-gradient(
      circle at 85% 15%,
      rgba(101, 67, 33, 0.08) 0%,
      transparent 12%
    ),
    radial-gradient(
      circle at 70% 80%,
      rgba(160, 82, 45, 0.06) 0%,
      transparent 8%
    ),
    radial-gradient(
      circle at 30% 90%,
      rgba(139, 69, 19, 0.07) 0%,
      transparent 15%
    ),
    /* Paper fiber texture */
      repeating-linear-gradient(
        23deg,
        transparent,
        transparent 1px,
        rgba(139, 69, 19, 0.02) 1px,
        rgba(139, 69, 19, 0.02) 2px,
        transparent 2px,
        transparent 8px
      ),
    repeating-linear-gradient(
      67deg,
      transparent,
      transparent 1px,
      rgba(101, 67, 33, 0.015) 1px,
      rgba(101, 67, 33, 0.015) 2px,
      transparent 2px,
      transparent 12px
    );
  border-radius: 8px;
  box-shadow: 
    /* Outer shadows for depth */ 0 25px 50px
      rgba(62, 39, 35, 0.4),
    0 15px 25px rgba(0, 0, 0, 0.2),
    /* Inner shadows for paper curl effect */ inset 0 1px 0
      rgba(255, 248, 235, 0.8),
    inset 0 -1px 0 rgba(139, 69, 19, 0.1),
    inset 1px 0 0 rgba(255, 248, 235, 0.6),
    inset -1px 0 0 rgba(139, 69, 19, 0.1);
  overflow: hidden;
  border: 1px solid rgba(139, 69, 19, 0.3);
  position: relative;
  transform: rotate(0.5deg);
}

.atm-frame::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    /* Burn marks and age stains */ radial-gradient(
      ellipse at 5% 10%,
      rgba(101, 67, 33, 0.08) 0%,
      transparent 20%
    ),
    radial-gradient(
      ellipse at 95% 5%,
      rgba(139, 69, 19, 0.06) 0%,
      transparent 15%
    ),
    radial-gradient(
      ellipse at 90% 95%,
      rgba(160, 82, 45, 0.05) 0%,
      transparent 18%
    ),
    radial-gradient(
      ellipse at 15% 85%,
      rgba(139, 69, 19, 0.04) 0%,
      transparent 12%
    ),
    /* Subtle paper texture */
      url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" width="200" height="200" viewBox="0 0 200 200"><defs><filter id="paper"><feTurbulence baseFrequency="0.04" numOctaves="5" result="noise" seed="1"/><feDiffuseLighting in="noise" lighting-color="white" surfaceScale="1"><feDistantLight azimuth="45" elevation="60"/></feDiffuseLighting></filter></defs><rect width="100%" height="100%" filter="url(%23paper)" opacity="0.3"/></svg>');
  opacity: 0.7;
  pointer-events: none;
}

.atm-frame::after {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    /* Wrinkles and creases */ linear-gradient(
      45deg,
      transparent 48%,
      rgba(139, 69, 19, 0.03) 49%,
      rgba(139, 69, 19, 0.03) 51%,
      transparent 52%
    ),
    linear-gradient(
      -45deg,
      transparent 48%,
      rgba(101, 67, 33, 0.02) 49%,
      rgba(101, 67, 33, 0.02) 51%,
      transparent 52%
    );
  pointer-events: none;
}

.atm-header {
  background: 
    /* Aged parchment header */ radial-gradient(
      ellipse at center,
      #e8d5b7 0%,
      #d4b896 50%,
      #c9a876 100%
    ),
    /* Subtle ink stains */
      radial-gradient(
        circle at 20% 50%,
        rgba(101, 67, 33, 0.1) 0%,
        transparent 15%
      ),
    radial-gradient(
      circle at 80% 30%,
      rgba(139, 69, 19, 0.08) 0%,
      transparent 12%
    );
  padding: 25px;
  text-align: center;
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  border-bottom: 2px solid rgba(139, 69, 19, 0.2);
  box-shadow: inset 0 -2px 4px rgba(139, 69, 19, 0.1),
    inset 0 2px 4px rgba(255, 248, 235, 0.3);
}

.atm-header::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    /* Parchment texture lines */ repeating-linear-gradient(
    90deg,
    transparent,
    transparent 20px,
    rgba(139, 69, 19, 0.02) 20px,
    rgba(139, 69, 19, 0.02) 21px,
    transparent 21px,
    transparent 40px
  );
  pointer-events: none;
}

.logo-container {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 15px;
}

.gringotts-logo {
  max-height: 80px;
  max-width: 80px;
  object-fit: contain;
  filter: sepia(0.3) saturate(1.2) brightness(1.1);
}

.goblin-seal-fallback {
  font-size: 3rem;
  color: #8b4513;
  text-shadow: 2px 2px 4px rgba(139, 69, 19, 0.3);
}

.bank-title {
  margin: 0;
  color: #2f1b14;
  text-align: center;
}

.gold-text {
  font-size: 2.5rem;
  font-weight: 700;
  font-family: "Cinzel", serif;
  display: block;
  text-shadow: 2px 2px 4px rgba(139, 69, 19, 0.3);
  letter-spacing: 3px;
  margin-bottom: 5px;
}

.subtitle {
  font-size: 1rem;
  font-weight: 400;
  font-family: "Cinzel", serif;
  letter-spacing: 1px;
  color: #5d4037;
}

.atm-screen {
  background: 
    /* Main parchment background with age variation */ radial-gradient(
      ellipse at 30% 40%,
      #f0e2d1 0%,
      #e5d0b8 30%,
      #d8c3a0 60%,
      #ccb188 100%
    ),
    /* Age spots and stains */
      radial-gradient(
        circle at 15% 25%,
        rgba(139, 69, 19, 0.06) 0%,
        transparent 12%
      ),
    radial-gradient(
      circle at 85% 70%,
      rgba(101, 67, 33, 0.05) 0%,
      transparent 15%
    ),
    radial-gradient(
      circle at 60% 10%,
      rgba(160, 82, 45, 0.04) 0%,
      transparent 10%
    ),
    /* Paper fiber texture */
      repeating-linear-gradient(
        17deg,
        transparent,
        transparent 1px,
        rgba(139, 69, 19, 0.015) 1px,
        rgba(139, 69, 19, 0.015) 2px,
        transparent 2px,
        transparent 10px
      );
  min-height: 500px;
  padding: 40px;
  color: #2f1b14;
  font-family: "Cinzel", serif;
  position: relative;
  box-shadow: inset 0 2px 8px rgba(139, 69, 19, 0.08),
    inset 0 -2px 4px rgba(255, 248, 235, 0.2);
}

.atm-screen::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    /* Parchment wear patterns */ radial-gradient(
      ellipse at 10% 90%,
      rgba(139, 69, 19, 0.03) 0%,
      transparent 25%
    ),
    radial-gradient(
      ellipse at 90% 10%,
      rgba(101, 67, 33, 0.025) 0%,
      transparent 20%
    ),
    /* Fine paper texture */
      url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" width="100" height="100" viewBox="0 0 100 100"><defs><filter id="paperTexture"><feTurbulence baseFrequency="0.3" numOctaves="4" result="noise" seed="2"/><feDiffuseLighting in="noise" lighting-color="%23f5f5dc" surfaceScale="0.5"><feDistantLight azimuth="45" elevation="60"/></feDiffuseLighting></filter></defs><rect width="100%" height="100%" filter="url(%23paperTexture)" opacity="0.2"/></svg>');
  pointer-events: none;
  opacity: 0.8;
}

.screen {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  position: relative;
  z-index: 1;
}

.navigation-bar {
  position: sticky;
  top: 0;
  width: 100%;
  background: 
    /* Darker parchment for navigation */ radial-gradient(
      ellipse at center,
      #e0d1be 0%,
      #d4c1a3 50%,
      #c8b186 100%
    ),
    radial-gradient(
      circle at 70% 30%,
      rgba(139, 69, 19, 0.06) 0%,
      transparent 15%
    );
  padding: 15px 20px;
  margin: -40px -40px 20px -40px;
  border-bottom: 1px solid rgba(139, 69, 19, 0.2);
  box-shadow: 0 2px 4px rgba(139, 69, 19, 0.1),
    inset 0 1px 2px rgba(255, 248, 235, 0.3);
  display: flex;
  justify-content: space-between;
  align-items: center;
  z-index: 10;
}

.nav-btn {
  background: radial-gradient(
    ellipse at 40% 30%,
    #e8d5b7 0%,
    #dcc899 50%,
    #cfb58a 100%
  );
  color: #2f1b14;
  border: 1px solid rgba(139, 69, 19, 0.25);
  padding: 8px 16px;
  border-radius: 4px;
  font-family: "Cinzel", serif;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  text-shadow: 1px 1px 1px rgba(139, 69, 19, 0.1);
  box-shadow: 0 2px 4px rgba(139, 69, 19, 0.1),
    inset 0 1px 2px rgba(255, 248, 235, 0.3);
}

.nav-btn:hover {
  transform: translateY(-1px);
  background: radial-gradient(
    ellipse at 40% 30%,
    #ebd8ba 0%,
    #dfc99c 50%,
    #d2b88d 100%
  );
  box-shadow: 0 3px 6px rgba(139, 69, 19, 0.15),
    inset 0 1px 2px rgba(255, 248, 235, 0.4);
}

.current-screen-title {
  color: #2f1b14;
  font-family: "Cinzel", serif;
  font-weight: 600;
  font-size: 1.1rem;
  text-shadow: 1px 1px 2px rgba(139, 69, 19, 0.15);
}

.welcome-msg {
  color: #2f1b14;
  font-size: 2.2rem;
  font-weight: 600;
  font-family: "Cinzel", serif;
  margin-bottom: 30px;
  text-shadow: 1px 1px 2px rgba(139, 69, 19, 0.2);
  letter-spacing: 1px;
}

.account-info {
  display: flex;
  gap: 25px;
  margin-bottom: 40px;
  flex-wrap: wrap;
  justify-content: center;
}

.account-card {
  background: linear-gradient(145deg, #f9f5e7, #f0e6ce);
  background-image: radial-gradient(
    circle at 30% 30%,
    rgba(218, 165, 32, 0.05) 0%,
    transparent 50%
  );
  padding: 25px;
  border-radius: 12px;
  border: 2px solid #8b4513;
  min-width: 220px;
  box-shadow: 0 4px 8px rgba(139, 69, 19, 0.2),
    inset 0 1px 2px rgba(255, 255, 255, 0.3);
  position: relative;
}

.account-card::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-image: repeating-linear-gradient(
    30deg,
    transparent,
    transparent 1px,
    rgba(139, 69, 19, 0.02) 1px,
    rgba(139, 69, 19, 0.02) 2px
  );
  border-radius: 12px;
  pointer-events: none;
}

.account-card h3 {
  color: #2f1b14;
  font-family: "Cinzel", serif;
  font-weight: 600;
  font-size: 1.2rem;
  margin: 0 0 10px 0;
  text-shadow: 1px 1px 2px rgba(139, 69, 19, 0.1);
  position: relative;
  z-index: 1;
}

.balance {
  font-size: 1.6rem;
  color: #8b4513;
  font-weight: 700;
  font-family: "Cinzel", serif;
  margin: 0;
  text-shadow: 1px 1px 2px rgba(139, 69, 19, 0.2);
  position: relative;
  z-index: 1;
}

.menu-options {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
  width: 100%;
  max-width: 600px;
}

.menu-btn {
  background: 
    /* Aged parchment with subtle variations */ radial-gradient(
      ellipse at 40% 30%,
      #f2e5d3 0%,
      #e6d2b5 40%,
      #dac497 80%,
      #ccb188 100%
    ),
    /* Age spots and ink marks */
      radial-gradient(
        circle at 20% 70%,
        rgba(139, 69, 19, 0.04) 0%,
        transparent 15%
      ),
    radial-gradient(
      circle at 80% 20%,
      rgba(101, 67, 33, 0.03) 0%,
      transparent 12%
    );
  color: #2f1b14;
  border: 1px solid rgba(139, 69, 19, 0.3);
  padding: 20px;
  border-radius: 6px;
  font-weight: 600;
  font-family: "Cinzel", serif;
  font-size: 1.1rem;
  cursor: pointer;
  transition: all 0.3s ease;
  text-shadow: 1px 1px 2px rgba(139, 69, 19, 0.15);
  box-shadow: 0 4px 8px rgba(139, 69, 19, 0.15),
    inset 0 1px 2px rgba(255, 248, 235, 0.4),
    inset 0 -1px 2px rgba(139, 69, 19, 0.08);
  position: relative;
  overflow: hidden;
}

.menu-btn::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    /* Paper fiber texture */ repeating-linear-gradient(
      23deg,
      transparent,
      transparent 1px,
      rgba(139, 69, 19, 0.015) 1px,
      rgba(139, 69, 19, 0.015) 2px,
      transparent 2px,
      transparent 8px
    ),
    /* Subtle wear marks */
      radial-gradient(
        ellipse at 90% 10%,
        rgba(160, 82, 45, 0.02) 0%,
        transparent 20%
      );
  pointer-events: none;
}

.menu-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(139, 69, 19, 0.2),
    inset 0 2px 4px rgba(255, 248, 235, 0.5),
    inset 0 -2px 4px rgba(139, 69, 19, 0.1);
  background: radial-gradient(
      ellipse at 40% 30%,
      #f5e8d6 0%,
      #e9d5b8 40%,
      #ddca9a 80%,
      #cfb48b 100%
    ),
    radial-gradient(
      circle at 20% 70%,
      rgba(139, 69, 19, 0.06) 0%,
      transparent 15%
    ),
    radial-gradient(
      circle at 80% 20%,
      rgba(101, 67, 33, 0.05) 0%,
      transparent 12%
    );
}

.account-selector {
  margin-bottom: 35px;
  width: 100%;
}

.account-selector label {
  display: block;
  color: #2f1b14;
  font-family: "Cinzel", serif;
  font-weight: 600;
  margin-bottom: 15px;
  font-size: 1.2rem;
  text-shadow: 1px 1px 2px rgba(139, 69, 19, 0.1);
}

.account-options {
  display: flex;
  gap: 20px;
  justify-content: center;
  flex-wrap: wrap;
}

.account-options.vertical {
  flex-direction: column;
  max-width: 250px;
}

.account-btn {
  background: 
    /* Aged parchment with subtle variations */ radial-gradient(
      ellipse at 35% 25%,
      #f0e3d1 0%,
      #e4d0b3 40%,
      #d8c395 80%,
      #cbb186 100%
    ),
    /* Age spots */
      radial-gradient(
        circle at 15% 60%,
        rgba(139, 69, 19, 0.03) 0%,
        transparent 12%
      ),
    radial-gradient(
      circle at 85% 25%,
      rgba(101, 67, 33, 0.025) 0%,
      transparent 10%
    );
  color: #2f1b14;
  border: 1px solid rgba(139, 69, 19, 0.25);
  padding: 18px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
  min-width: 140px;
  font-family: "Cinzel", serif;
  font-weight: 500;
  text-shadow: 1px 1px 2px rgba(139, 69, 19, 0.12);
  box-shadow: 0 3px 6px rgba(139, 69, 19, 0.12),
    inset 0 1px 2px rgba(255, 248, 235, 0.35),
    inset 0 -1px 2px rgba(139, 69, 19, 0.06);
  position: relative;
  overflow: hidden;
}

.account-btn::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    /* Paper texture */ repeating-linear-gradient(
    17deg,
    transparent,
    transparent 1px,
    rgba(139, 69, 19, 0.01) 1px,
    rgba(139, 69, 19, 0.01) 2px,
    transparent 2px,
    transparent 10px
  );
  pointer-events: none;
}

.account-btn:hover {
  background: radial-gradient(
      ellipse at 35% 25%,
      #f3e6d4 0%,
      #e7d3b6 40%,
      #dbc698 80%,
      #ceb489 100%
    ),
    radial-gradient(
      circle at 15% 60%,
      rgba(139, 69, 19, 0.05) 0%,
      transparent 12%
    ),
    radial-gradient(
      circle at 85% 25%,
      rgba(101, 67, 33, 0.04) 0%,
      transparent 10%
    );
  box-shadow: 0 4px 8px rgba(139, 69, 19, 0.18),
    inset 0 2px 4px rgba(255, 248, 235, 0.4),
    inset 0 -2px 4px rgba(139, 69, 19, 0.08);
}

.account-btn.active {
  border-color: rgba(139, 69, 19, 0.5);
  background: radial-gradient(
      ellipse at 35% 25%,
      #e6d3a3 0%,
      #dac497 40%,
      #ccb188 80%,
      #c0a373 100%
    ),
    radial-gradient(
      circle at 15% 60%,
      rgba(139, 69, 19, 0.08) 0%,
      transparent 12%
    ),
    radial-gradient(
      circle at 85% 25%,
      rgba(101, 67, 33, 0.06) 0%,
      transparent 10%
    );
  box-shadow: inset 0 2px 6px rgba(139, 69, 19, 0.15),
    inset 0 -1px 3px rgba(255, 248, 235, 0.2), 0 2px 4px rgba(139, 69, 19, 0.1);
}

.amount-section {
  width: 100%;
  max-width: 400px;
}

.available-balance {
  color: #ffd700;
  font-size: 1.1rem;
  margin-bottom: 20px;
}

.quick-amounts {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
  margin-bottom: 20px;
}

.amount-btn {
  background: linear-gradient(45deg, #8a2be2, #9370db);
  color: white;
  border: none;
  padding: 15px;
  border-radius: 5px;
  cursor: pointer;
  font-weight: bold;
  transition: all 0.3s ease;
}

.amount-btn:hover:not(:disabled) {
  background: linear-gradient(45deg, #9370db, #8a2be2);
}

.amount-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.custom-amount {
  margin-bottom: 20px;
}

.custom-amount label {
  display: block;
  color: #2f1b14;
  font-family: "Cinzel", serif;
  font-weight: 600;
  margin-bottom: 10px;
  font-size: 1.1rem;
  text-shadow: 1px 1px 2px rgba(139, 69, 19, 0.1);
}

.magical-input {
  width: 100%;
  padding: 15px;
  border: 1px solid rgba(139, 69, 19, 0.3);
  border-radius: 4px;
  background: 
    /* Parchment input background */ radial-gradient(
      ellipse at 30% 30%,
      #f5e8d6 0%,
      #e9d5b8 50%,
      #ddc99a 100%
    ),
    /* Subtle age marks */
      radial-gradient(
        circle at 80% 20%,
        rgba(139, 69, 19, 0.02) 0%,
        transparent 15%
      ),
    radial-gradient(
      circle at 20% 80%,
      rgba(101, 67, 33, 0.015) 0%,
      transparent 10%
    );
  color: #2f1b14;
  font-family: "Cinzel", serif;
  font-size: 1rem;
  font-weight: 500;
  box-shadow: inset 0 2px 4px rgba(139, 69, 19, 0.08),
    inset 0 -1px 2px rgba(255, 248, 235, 0.3), 0 1px 2px rgba(139, 69, 19, 0.05);
  transition: all 0.3s ease;
  position: relative;
}

.magical-input::placeholder {
  color: rgba(47, 27, 20, 0.6);
  font-style: italic;
}

.magical-input:focus {
  outline: none;
  border-color: rgba(93, 64, 55, 0.5);
  box-shadow: inset 0 2px 6px rgba(139, 69, 19, 0.12),
    inset 0 -1px 2px rgba(255, 248, 235, 0.4), 0 0 8px rgba(139, 69, 19, 0.15),
    0 2px 4px rgba(139, 69, 19, 0.08);
  background: radial-gradient(
      ellipse at 30% 30%,
      #f8ebda 0%,
      #ecd8bb 50%,
      #e0ce9d 100%
    ),
    radial-gradient(
      circle at 80% 20%,
      rgba(139, 69, 19, 0.03) 0%,
      transparent 15%
    ),
    radial-gradient(
      circle at 20% 80%,
      rgba(101, 67, 33, 0.025) 0%,
      transparent 10%
    );
}

.button-group {
  display: flex;
  gap: 15px;
  justify-content: center;
  flex-wrap: wrap;
}

.magical-btn {
  background: 
    /* Aged parchment with subtle variations */ radial-gradient(
      ellipse at 40% 30%,
      #f0e3d1 0%,
      #e4d0b3 40%,
      #d8c395 80%,
      #cbb186 100%
    ),
    /* Age spots */
      radial-gradient(
        circle at 25% 70%,
        rgba(139, 69, 19, 0.04) 0%,
        transparent 15%
      ),
    radial-gradient(
      circle at 75% 20%,
      rgba(101, 67, 33, 0.03) 0%,
      transparent 12%
    );
  color: #2f1b14;
  border: 1px solid rgba(139, 69, 19, 0.3);
  padding: 15px 28px;
  border-radius: 6px;
  font-weight: 600;
  font-family: "Cinzel", serif;
  cursor: pointer;
  font-size: 1.1rem;
  transition: all 0.3s ease;
  text-shadow: 1px 1px 2px rgba(139, 69, 19, 0.15);
  box-shadow: 0 4px 8px rgba(139, 69, 19, 0.15),
    inset 0 1px 2px rgba(255, 248, 235, 0.4),
    inset 0 -1px 2px rgba(139, 69, 19, 0.08);
  position: relative;
  overflow: hidden;
}

.magical-btn::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    /* Paper fiber texture */ repeating-linear-gradient(
    25deg,
    transparent,
    transparent 1px,
    rgba(139, 69, 19, 0.015) 1px,
    rgba(139, 69, 19, 0.015) 2px,
    transparent 2px,
    transparent 8px
  );
  pointer-events: none;
}

.magical-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  background: radial-gradient(
      ellipse at 40% 30%,
      #f3e6d4 0%,
      #e7d3b6 40%,
      #dbc698 80%,
      #ceb489 100%
    ),
    radial-gradient(
      circle at 25% 70%,
      rgba(139, 69, 19, 0.06) 0%,
      transparent 15%
    ),
    radial-gradient(
      circle at 75% 20%,
      rgba(101, 67, 33, 0.05) 0%,
      transparent 12%
    );
  box-shadow: 0 6px 12px rgba(139, 69, 19, 0.2),
    inset 0 2px 4px rgba(255, 248, 235, 0.5),
    inset 0 -2px 4px rgba(139, 69, 19, 0.1);
}

.magical-btn.primary {
  background: radial-gradient(
      ellipse at 40% 30%,
      #e6d3a3 0%,
      #dac497 40%,
      #ccb188 80%,
      #c0a373 100%
    ),
    radial-gradient(
      circle at 25% 70%,
      rgba(139, 69, 19, 0.06) 0%,
      transparent 15%
    ),
    radial-gradient(
      circle at 75% 20%,
      rgba(101, 67, 33, 0.05) 0%,
      transparent 12%
    );
  border-color: rgba(139, 69, 19, 0.4);
  color: #2f1b14;
}

.magical-btn.primary:hover:not(:disabled) {
  background: radial-gradient(
      ellipse at 40% 30%,
      #f0e6ce 0%,
      #e6d3a3 40%,
      #dac497 80%,
      #ccb188 100%
    ),
    radial-gradient(
      circle at 25% 70%,
      rgba(139, 69, 19, 0.08) 0%,
      transparent 15%
    ),
    radial-gradient(
      circle at 75% 20%,
      rgba(101, 67, 33, 0.06) 0%,
      transparent 12%
    );
}

.magical-btn.secondary {
  background: radial-gradient(
      ellipse at 40% 30%,
      #d2b48c 0%,
      #ccb188 40%,
      #c0a373 80%,
      #b8985e 100%
    ),
    radial-gradient(
      circle at 25% 70%,
      rgba(139, 69, 19, 0.08) 0%,
      transparent 15%
    ),
    radial-gradient(
      circle at 75% 20%,
      rgba(101, 67, 33, 0.06) 0%,
      transparent 12%
    );
  border-color: rgba(139, 69, 19, 0.4);
  color: #2f1b14;
}

.magical-btn.secondary:hover:not(:disabled) {
  background: radial-gradient(
      ellipse at 40% 30%,
      #e6d3a3 0%,
      #d2b48c 40%,
      #ccb188 80%,
      #c0a373 100%
    ),
    radial-gradient(
      circle at 25% 70%,
      rgba(139, 69, 19, 0.1) 0%,
      transparent 15%
    ),
    radial-gradient(
      circle at 75% 20%,
      rgba(101, 67, 33, 0.08) 0%,
      transparent 12%
    );
}

.magical-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
  background: linear-gradient(145deg, #e0e0e0, #d0d0d0);
  color: #888;
  border-color: #999;
}

.transfer-section {
  width: 100%;
  max-width: 600px;
}

.transfer-accounts {
  display: flex;
  align-items: center;
  gap: 20px;
  margin-bottom: 30px;
  flex-wrap: wrap;
  justify-content: center;
}

.from-account,
.to-account {
  flex: 1;
  min-width: 200px;
}

.from-account label,
.to-account label {
  display: block;
  color: #ffd700;
  margin-bottom: 10px;
  font-weight: bold;
}

.transfer-arrow {
  font-size: 2rem;
  color: #ffd700;
}

.transaction-list {
  width: 100%;
  max-width: 500px;
  max-height: 400px;
  overflow-y: auto;
  margin-bottom: 20px;
}

.transaction-item {
  background: 
    /* Parchment card background */ radial-gradient(
      ellipse at 35% 25%,
      #ede0ce 0%,
      #e1ceb1 40%,
      #d5c294 80%,
      #c8b187 100%
    ),
    /* Age spots and wear */
      radial-gradient(
        circle at 15% 70%,
        rgba(139, 69, 19, 0.04) 0%,
        transparent 15%
      ),
    radial-gradient(
      circle at 85% 30%,
      rgba(101, 67, 33, 0.03) 0%,
      transparent 12%
    );
  border: 1px solid rgba(139, 69, 19, 0.2);
  border-radius: 6px;
  padding: 15px;
  margin-bottom: 10px;
  box-shadow: 0 2px 4px rgba(139, 69, 19, 0.1),
    inset 0 1px 2px rgba(255, 248, 235, 0.3);
  position: relative;
}

.transaction-item::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    /* Fine paper texture */ repeating-linear-gradient(
    19deg,
    transparent,
    transparent 1px,
    rgba(139, 69, 19, 0.01) 1px,
    rgba(139, 69, 19, 0.01) 2px,
    transparent 2px,
    transparent 12px
  );
  pointer-events: none;
  border-radius: 6px;
}

.transaction-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.transaction-type {
  color: #8b4513;
  font-weight: bold;
  text-shadow: 1px 1px 2px rgba(139, 69, 19, 0.1);
}

.transaction-date {
  color: #654321;
  font-size: 0.9rem;
  text-shadow: 1px 1px 1px rgba(139, 69, 19, 0.05);
}

.transaction-details {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.transaction-amount {
  font-weight: bold;
  font-size: 1.1rem;
  color: #2f1b14;
  text-shadow: 1px 1px 2px rgba(139, 69, 19, 0.1);
}

.transaction-amount.positive {
  color: #2d5a2d;
}

.transaction-amount.negative {
  color: #ff6347;
}

.transaction-account {
  color: #87ceeb;
}

.no-transactions {
  color: #666;
  font-style: italic;
  margin: 40px 0;
}

.result-screen {
  justify-content: center;
}

.success {
  color: #32cd32;
  text-align: center;
}

.error {
  color: #ff6347;
  text-align: center;
}

.updated-balances {
  margin: 20px 0;
}

.balance-update {
  color: #ffd700;
  font-size: 1.1rem;
  margin: 5px 0;
}

.loading-screen {
  justify-content: center;
}

.spinner {
  font-size: 3rem;
  animation: spin 1s linear infinite;
  margin-bottom: 20px;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.atm-footer {
  background: linear-gradient(145deg, #e6d3a3, #d2b48c);
  background-image: radial-gradient(
    circle at 40% 40%,
    rgba(139, 69, 19, 0.03) 0%,
    transparent 50%
  );
  padding: 18px;
  text-align: center;
  border-top: 2px solid #8b4513;
  position: relative;
}

.atm-footer::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-image: repeating-linear-gradient(
    60deg,
    transparent,
    transparent 1px,
    rgba(139, 69, 19, 0.02) 1px,
    rgba(139, 69, 19, 0.02) 2px
  );
  pointer-events: none;
}

.security-notice {
  color: #5d4037;
  font-size: 1rem;
  font-family: "Cinzel", serif;
  font-weight: 500;
  margin: 0;
  font-style: italic;
  text-shadow: 1px 1px 2px rgba(139, 69, 19, 0.1);
  position: relative;
  z-index: 1;
}

@media (max-width: 768px) {
  .atm-frame {
    width: 95%;
  }

  .atm-screen {
    padding: 20px;
  }

  .menu-options {
    grid-template-columns: 1fr;
  }

  .account-info {
    flex-direction: column;
  }

  .transfer-accounts {
    flex-direction: column;
  }

  .transfer-arrow {
    transform: rotate(90deg);
  }

  .quick-amounts {
    grid-template-columns: 1fr;
  }
}
</style>
