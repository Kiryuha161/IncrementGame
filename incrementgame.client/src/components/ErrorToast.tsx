import styles from '../App.module.css';

interface ErrorToastProps {
    error: string | null;
    show: boolean;
    onClose: () => void;
}

export function ErrorToast({ error, show, onClose }: ErrorToastProps) {
    if (!show || !error) return null;

    return (
        <div className={styles.errorToast}>
            <div className={styles.errorToastHeader}>
                <span className={styles.errorToastTitle}>
                    <span>⚠️</span> Ошибка
                </span>
                <button
                    className={styles.errorToastClose}
                    onClick={onClose}
                >
                    ✕
                </button>
            </div>
            <div className={styles.errorToastMessage}>
                {error}
            </div>
        </div>
    );
}