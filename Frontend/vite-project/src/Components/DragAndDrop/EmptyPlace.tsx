import { useDrop } from 'react-dnd';
import { ItemTypes } from './ItemTypes';
import { useRef } from 'react';

interface EmptyPlaceProps {
    index: number;
    word: string;
    onRemoveWord: (index: number) => void;
    onDrop: (index: number, word: string) => void;
}

export default function EmptyPlace({
    index,
    word,
    onRemoveWord,
    onDrop
}: EmptyPlaceProps) {
    const ref = useRef<HTMLSpanElement>(null);

    const [{ isOver }, drop] = useDrop({
        accept: ItemTypes.WORD,
        drop: (item: any) => {
            console.log('Dropped item:', item);
            onDrop(index, item.word);
            return { dropped: true };
        },
        collect: (monitor) => ({
            isOver: !!monitor.isOver(),
            canDrop: !!monitor.canDrop()
        })
    });
    drop(ref);

    const handleClick = () => {
        if (word) {
            onRemoveWord(index);
        }
    };

    return (
        <span
            ref={ref}
            onClick={handleClick}
            className={`min-w-[50px] min-h-[30px]
inline-block rounded-md px-3 py-1 mx-1 border cursor-pointer ${word ? 'bg-blue-100 text-blue-700 border-blue-300' :
                    isOver ? 'bg-green-100 border-green-300' : 'bg-gray-200 text-gray-500 border-gray-300'
                }`}
        >
            {word || "___"}
        </span>
    );
}