class Modal {
    private modalOverlay: HTMLDivElement;
    private modalContainer: HTMLDivElement;
    private content: HTMLDivElement;
    private closeButton: HTMLButtonElement;

    constructor() {
        // Фон, що затемнює основний контент
        this.modalOverlay = document.createElement('div');
        this.modalOverlay.classList.add('modal-overlay');

        // Контейнер модального вікна
        this.modalContainer = document.createElement('div');
        this.modalContainer.classList.add('modal-container');

        // Вміст модального вікна
        this.content = document.createElement('div');
        this.content.classList.add('modal-content');

        // Кнопка закриття
        this.closeButton = document.createElement('button');
        this.closeButton.textContent = 'Закрити';
        this.closeButton.classList.add('modal-close');
        this.closeButton.addEventListener('click', () => this.close());

        // Додаємо елементи в модальне вікно
        this.content.appendChild(this.closeButton);
        this.modalContainer.appendChild(this.content);
        this.modalOverlay.appendChild(this.modalContainer);
    }

    open(content: string) {
        this.content.innerHTML = ''; // Очищуємо попередній вміст
        this.content.appendChild(this.closeButton); // Додаємо кнопку закриття
        this.content.insertAdjacentHTML('beforeend', `<p>${content}</p>`);
    
        document.body.appendChild(this.modalOverlay);
        document.body.style.overflow = 'hidden'; // Блокуємо скролінг
    }
    

    close() {
        this.modalOverlay.remove();
        document.body.style.overflow = ''; // Відновлюємо скролінг
    }
}

export default Modal;
