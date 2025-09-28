// API service for Gringotts Bank
// Use the container name for internal communication when running in Docker
const API_BASE =
  process.env.NODE_ENV === "development" && typeof window !== "undefined"
    ? "http://localhost:8080/api" // For browser/development
    : "http://gringottsbankapi:8080/api"; // For container-to-container communication

export class BankApiService {
  static async getAllAccounts() {
    try {
      const response = await fetch(`${API_BASE}/AccountInfo`);
      if (!response.ok)
        throw new Error(`HTTP error! status: ${response.status}`);
      return await response.json();
    } catch (error) {
      console.error("Failed to fetch accounts:", error);
      throw error;
    }
  }

  static async getAccount(id) {
    try {
      const response = await fetch(`${API_BASE}/AccountInfo/${id}`);
      if (!response.ok)
        throw new Error(`HTTP error! status: ${response.status}`);
      return await response.json();
    } catch (error) {
      console.error("Failed to fetch account:", error);
      throw error;
    }
  }

  static async createTransaction(transactionData) {
    try {
      const response = await fetch(`${API_BASE}/AccountTransactions`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(transactionData),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || `HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error("Failed to create transaction:", error);
      throw error;
    }
  }

  static async transferFunds(transferData) {
    try {
      const response = await fetch(`${API_BASE}/AccountTransactions/transfer`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(transferData),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || `HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error("Failed to transfer funds:", error);
      throw error;
    }
  }

  static async getTransactionsByAccount(accountId) {
    try {
      const response = await fetch(
        `${API_BASE}/AccountTransactions/account/${accountId}`
      );
      if (!response.ok)
        throw new Error(`HTTP error! status: ${response.status}`);
      return await response.json();
    } catch (error) {
      console.error("Failed to fetch transactions:", error);
      throw error;
    }
  }

  static async getAllTransactions() {
    try {
      const response = await fetch(`${API_BASE}/AccountTransactions`);
      if (!response.ok)
        throw new Error(`HTTP error! status: ${response.status}`);
      return await response.json();
    } catch (error) {
      console.error("Failed to fetch all transactions:", error);
      throw error;
    }
  }
}
