document.addEventListener('DOMContentLoaded', async () => {
    // Format currency
    const formatCurrency = (amount, currency = 'USD') => {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: currency
        }).format(amount);
    };

    // Format date
    const formatDate = (dateString) => {
        const options = { month: 'short', day: 'numeric', year: 'numeric' };
        return new Date(dateString).toLocaleDateString('en-US', options);
    };

    try {
        // Fetch Portfolio Data
        const portfolioRes = await fetch('/api/portfolio');
        const portfolioData = await portfolioRes.json();

        // Update KPIs
        document.getElementById('kpi-balance').textContent = formatCurrency(portfolioData.balance);
        document.getElementById('kpi-income').textContent = formatCurrency(portfolioData.monthlyIncome);
        document.getElementById('kpi-expenses').textContent = formatCurrency(portfolioData.monthlyExpenses);
        
        const trendEl = document.getElementById('kpi-trend');
        trendEl.innerHTML = `<span>↑</span> ${portfolioData.trend} this month`;

        // Render Chart
        const ctx = document.getElementById('balanceChart').getContext('2d');
        
        // Gradient fill
        const gradient = ctx.createLinearGradient(0, 0, 0, 400);
        gradient.addColorStop(0, 'rgba(59, 130, 246, 0.5)');
        gradient.addColorStop(1, 'rgba(59, 130, 246, 0.0)');

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
                datasets: [{
                    label: 'Balance',
                    data: portfolioData.chartData,
                    borderColor: '#3b82f6',
                    backgroundColor: gradient,
                    borderWidth: 3,
                    pointBackgroundColor: '#1a1d24',
                    pointBorderColor: '#3b82f6',
                    pointBorderWidth: 2,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        backgroundColor: '#1a1d24',
                        titleColor: '#9ca3af',
                        bodyColor: '#f3f4f6',
                        borderColor: '#2a2e39',
                        borderWidth: 1,
                        padding: 10,
                        displayColors: false,
                        callbacks: {
                            label: (context) => formatCurrency(context.raw)
                        }
                    }
                },
                scales: {
                    x: {
                        grid: { display: false, drawBorder: false },
                        ticks: { color: '#9ca3af', font: { family: 'Inter' } }
                    },
                    y: {
                        grid: { color: '#2a2e39', borderDash: [5, 5], drawBorder: false },
                        ticks: {
                            color: '#9ca3af',
                            font: { family: 'Inter' },
                            callback: (value) => '$' + value / 1000 + 'k'
                        }
                    }
                },
                interaction: {
                    intersect: false,
                    mode: 'index',
                },
            }
        });

        // Fetch Transactions Data
        const txRes = await fetch('/api/transactions');
        const txData = await txRes.json();

        // Render Transactions
        const txListEl = document.getElementById('transaction-list');
        txListEl.innerHTML = ''; // clear loading

        txData.forEach(tx => {
            const icon = tx.type === 'credit' ? '↓' : '↑';
            const amountClass = tx.type === 'credit' ? 'credit' : '';
            const amountPrefix = tx.type === 'credit' ? '+' : '';
            
            const txItem = document.createElement('div');
            txItem.className = 'tx-item';
            txItem.innerHTML = `
                <div class="tx-left">
                    <div class="tx-icon ${tx.type}">${icon}</div>
                    <div class="tx-info">
                        <div class="tx-desc">${tx.description}</div>
                        <div class="tx-date">${formatDate(tx.date)}</div>
                    </div>
                </div>
                <div class="tx-amount ${amountClass}">${amountPrefix}${formatCurrency(Math.abs(tx.amount))}</div>
            `;
            txListEl.appendChild(txItem);
        });

    } catch (error) {
        console.error("Error loading dashboard data:", error);
        document.getElementById('transaction-list').innerHTML = 
            '<div class="loading" style="color: #ef4444">Failed to load data. Please try again.</div>';
    }
});
