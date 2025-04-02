import Modal from './Modal';

class App {
    private container: HTMLElement;
    private modal: Modal;

    constructor() {
        this.container = document.body;
        this.modal = new Modal();
    }

    run() {
        const button = document.createElement('button');
        button.textContent = 'Відкрити модальне вікно';

        button.addEventListener('click', () => {
            this.modal.open('Це контент модального вікна');
        });

        this.container.append(button);
    }
}

export default App;
