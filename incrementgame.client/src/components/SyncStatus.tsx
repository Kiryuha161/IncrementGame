import styles from '../App.module.css';

interface SyncStatusProps {
    status: 'synced' | 'syncing' | 'error';
}

export function SyncStatus({ status }: SyncStatusProps) {
    const statusClass =
        status === 'syncing' ? styles.syncing :
            status === 'synced' ? styles.synced :
                styles.error;

    const statusText =
        status === 'syncing' ? '⏳ Сохранение...' :
            status === 'synced' ? '💾 Все сохранено' :
                '⚠️ Ошибка сохранения';

    return (
        <div className={`${styles.syncStatus} ${statusClass}`}>
            {statusText}
        </div>
    );
}