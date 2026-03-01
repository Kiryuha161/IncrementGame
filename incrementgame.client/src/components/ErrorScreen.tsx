import styles from '../App.module.css';

interface ErrorScreenProps {
    error: string;
    onRetry: () => void;
}

export function ErrorScreen({ error, onRetry }: ErrorScreenProps) {
    return (
        <div className={styles.errorContainer}>
            <div className={styles.errorIcon}>😵</div>
            <div className={styles.errorTitle}>Упс! Что-то пошло не так</div>
            <div className={styles.errorMessage}>{error}</div>
            <button
                className={styles.errorRetryButton}
                onClick={onRetry}
            >
                Попробовать снова
            </button>
        </div>
    );
}