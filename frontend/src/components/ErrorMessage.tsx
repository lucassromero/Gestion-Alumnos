export function ErrorMessage({
  titulo = 'Ocurrió un error',
  mensaje,
  onReintentar,
}: {
  titulo?: string
  mensaje: string
  onReintentar?: () => void
}) {
  return (
    <div className="rounded border border-error/20 bg-error/5 p-4 text-error">
      <h2 className="font-serif text-lg font-medium">{titulo}</h2>
      <p className="mt-1 text-sm">{mensaje}</p>
      {onReintentar && (
        <button
          type="button"
          onClick={onReintentar}
          className="mt-4 rounded border border-error px-3 py-1.5 text-sm font-medium hover:bg-error hover:text-hoja"
        >
          Reintentar
        </button>
      )}
    </div>
  )
}
