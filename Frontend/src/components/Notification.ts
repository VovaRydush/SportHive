export class NotificationKarina {
  private container: HTMLElement;

  constructor() {
    this.container = document.createElement('div');
    this.container.className = 'notification-container';
    document.body.appendChild(this.container);
  }

  show(message: string, type: 'success' | 'error' | 'info' = 'info') {
    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.innerHTML = `
      <div class="notification-content">${message}</div>
      <div class="progress-bar"></div>
    `;
    
    this.container.appendChild(notification);
    
    // Анімація прогресу
    const progressBar = notification.querySelector('.progress-bar') as HTMLElement;
    progressBar.style.animation = 'progress 5s linear forwards';
    
    // Автоматичне зникнення
    setTimeout(() => {
      notification.classList.add('fade-out');
      setTimeout(() => notification.remove(), 300);
    }, 5000);
  }
}

// Стилі для сповіщення
const notificationStyles = document.createElement('style');
notificationStyles.textContent = `
  .notification-container {
    position: fixed;
    top: 20px;
    right: 20px;
    z-index: 1000;
    display: flex;
    flex-direction: column;
    gap: 10px;
  }

  .notification {
    padding: 15px 20px;
    border-radius: 4px;
    background: #111;
    color: white;
    font-family: 'Montserrat', sans-serif;
    box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    transform: translateX(0);
    opacity: 1;
    transition: all 0.3s ease;
    max-width: 300px;
    border-left: 4px solid;
    overflow: hidden;
  }

  .notification.success {
    border-left-color: #2e7d32;
  }

  .notification.error {
    border-left-color: #c62828;
  }

  .notification.info {
    border-left-color: #1565c0;
  }

  .notification-content {
    margin-bottom: 8px;
  }

  .progress-bar {
    height: 3px;
    background: rgba(255, 255, 255, 0.3);
    width: 100%;
    border-radius: 2px;
    transform-origin: left;
    animation: none;
  }

  @keyframes progress {
    0% { transform: scaleX(1); }
    100% { transform: scaleX(0); }
  }

  .notification.fade-out {
    transform: translateX(100%);
    opacity: 0;
  }

  @media (max-width: 600px) {
    .notification-container {
      top: 10px;
      right: 10px;
      left: 10px;
      align-items: center;
    }
    
    .notification {
      width: 100%;
      max-width: none;
    }
  }
`;
document.head.appendChild(notificationStyles);