import { useDrag } from 'react-dnd';
import { useRef } from 'react';
import { ItemTypes } from './ItemTypes';

export default function DraggableWord({ word }: { word: string }) {
    const ref = useRef<HTMLDivElement>(null);

    const [{ isDragging }, drag] = useDrag({
        type: ItemTypes.WORD,
        item: { type: ItemTypes.WORD, word },
        collect: (monitor) => ({
            isDragging: !!monitor.isDragging()
        })
    });
    drag(ref);

    return (
        <div
            ref={ref}
            className={`px-4 py-3 bg-blue-50 text-blue-700 rounded-md border-2 border-blue-200 shadow-sm cursor-move hover:shadow-md transition-all font-medium text-center min-w-20 ${isDragging ? 'opacity-50' : 'opacity-100'}`}
        >
            {word}
        </div>
    );
}