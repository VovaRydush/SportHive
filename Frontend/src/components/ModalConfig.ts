// DeleteConfirmationModal.ts
import './modalConfig.css';

export class DeleteConfirmationModal {
  private modalElement: HTMLElement;

  constructor() {
    this.modalElement = document.createElement('div');
    this.modalElement.className = 'delete-confirmation-modal';
    this.render();
  }

  public render() {
    this.modalElement.innerHTML = `
      <div class="modal-content">
        <div class="modal-header">
          <h3>Підтвердження вилучення</h3>
        </div>
        <div class="modal-body">
          <p>Ви впевнені, що хочете вилучити цього учасника з матчу?</p>
        </div>
        <div class="modal-footer">
          <button class="btn-cancel">Скасувати</button>
          <button class="btn btn-confirm">Вилучити</button>
        </div>
      </div>
    `;
    this.modalElement.classList.add('active');
this.setupEventListeners();

    document.body.appendChild(this.modalElement);
  }
   private setupEventListeners() {
    const confirmBtn = this.modalElement.querySelector('.btn-confirm');
    const cancelBtn = this.modalElement.querySelector('.btn-cancel');

    confirmBtn?.addEventListener('click', () => {
      this.close();
    });

    cancelBtn?.addEventListener('click', () => {
      this.close();
    });

    this.modalElement.addEventListener('click', (e) => {
      if (e.target === this.modalElement) {
        this.close();
      }
    });
  }
  public open() {
    this.modalElement.classList.add('active');
    document.body.style.overflow = 'hidden';
  }

  public close() {
    this.modalElement.classList.remove('active');
    document.body.style.overflow = '';
  }
}